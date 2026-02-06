#nullable enable

using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using OneGuru.CFR.Domain.Ports;

namespace OneGuru.CFR.Infrastructure.Messaging;

/// <summary>
/// Azure Service Bus implementation of IEventPublisher for event-driven communication.
/// </summary>
public class ServiceBusEventPublisher : IEventPublisher, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ILogger<ServiceBusEventPublisher> _logger;
    private readonly Dictionary<string, ServiceBusSender> _senders = new();
    private readonly SemaphoreSlim _senderLock = new(1, 1);

    public ServiceBusEventPublisher(string connectionString, ILogger<ServiceBusEventPublisher> logger)
    {
        _client = new ServiceBusClient(connectionString);
        _logger = logger;
    }

    public async Task PublishAsync<T>(string topicName, T eventPayload, string? correlationId = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var sender = await GetOrCreateSenderAsync(topicName);
            var message = CreateMessage(eventPayload, correlationId);

            await sender.SendMessageAsync(message, cancellationToken);

            _logger.LogInformation("Published event to topic {TopicName} with correlation ID {CorrelationId}",
                topicName, correlationId ?? "none");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event to topic {TopicName}", topicName);
            throw;
        }
    }

    public async Task SendToQueueAsync<T>(string queueName, T eventPayload, string? correlationId = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var sender = await GetOrCreateSenderAsync(queueName);
            var message = CreateMessage(eventPayload, correlationId);

            await sender.SendMessageAsync(message, cancellationToken);

            _logger.LogInformation("Sent message to queue {QueueName} with correlation ID {CorrelationId}",
                queueName, correlationId ?? "none");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message to queue {QueueName}", queueName);
            throw;
        }
    }

    public async Task PublishBatchAsync<T>(string topicName, IEnumerable<T> events, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var sender = await GetOrCreateSenderAsync(topicName);
            var eventList = events.ToList();

            using var batch = await sender.CreateMessageBatchAsync(cancellationToken);

            foreach (var eventPayload in eventList)
            {
                var message = CreateMessage(eventPayload, null);
                if (!batch.TryAddMessage(message))
                {
                    // Batch is full, send current batch and create a new one
                    await sender.SendMessagesAsync(batch, cancellationToken);

                    using var newBatch = await sender.CreateMessageBatchAsync(cancellationToken);
                    if (!newBatch.TryAddMessage(message))
                    {
                        throw new InvalidOperationException("Message is too large for the batch");
                    }
                    await sender.SendMessagesAsync(newBatch, cancellationToken);
                }
            }

            if (batch.Count > 0)
            {
                await sender.SendMessagesAsync(batch, cancellationToken);
            }

            _logger.LogInformation("Published batch of {Count} events to topic {TopicName}",
                eventList.Count, topicName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish batch to topic {TopicName}", topicName);
            throw;
        }
    }

    private ServiceBusMessage CreateMessage<T>(T payload, string? correlationId) where T : class
    {
        var json = JsonSerializer.Serialize(payload);
        var message = new ServiceBusMessage(json)
        {
            ContentType = "application/json",
            MessageId = Guid.NewGuid().ToString()
        };

        if (!string.IsNullOrEmpty(correlationId))
        {
            message.CorrelationId = correlationId;
        }

        message.ApplicationProperties["EventType"] = typeof(T).Name;
        message.ApplicationProperties["Timestamp"] = DateTime.UtcNow.ToString("O");

        return message;
    }

    private async Task<ServiceBusSender> GetOrCreateSenderAsync(string destination)
    {
        await _senderLock.WaitAsync();
        try
        {
            if (!_senders.TryGetValue(destination, out var sender))
            {
                sender = _client.CreateSender(destination);
                _senders[destination] = sender;
            }
            return sender;
        }
        finally
        {
            _senderLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var sender in _senders.Values)
        {
            await sender.DisposeAsync();
        }
        await _client.DisposeAsync();
        _senderLock.Dispose();
    }
}
