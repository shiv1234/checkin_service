#nullable enable

using Microsoft.Extensions.Logging;
using OneGuru.CFR.Domain.Ports;

namespace OneGuru.CFR.Infrastructure.Messaging;

/// <summary>
/// Null implementation of IEventPublisher for when Azure Service Bus is not configured.
/// Logs events but does not actually publish them.
/// </summary>
public class NullEventPublisher : IEventPublisher
{
    private readonly ILogger<NullEventPublisher> _logger;

    public NullEventPublisher(ILogger<NullEventPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<T>(string topicName, T eventPayload, string? correlationId = null, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogWarning("Azure Service Bus not configured. Event to topic {TopicName} was not published. " +
            "Configure 'AzureServiceBus:ConnectionString' to enable event publishing.", topicName);
        return Task.CompletedTask;
    }

    public Task SendToQueueAsync<T>(string queueName, T eventPayload, string? correlationId = null, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogWarning("Azure Service Bus not configured. Message to queue {QueueName} was not sent. " +
            "Configure 'AzureServiceBus:ConnectionString' to enable queue messaging.", queueName);
        return Task.CompletedTask;
    }

    public Task PublishBatchAsync<T>(string topicName, IEnumerable<T> events, CancellationToken cancellationToken = default) where T : class
    {
        var count = events.Count();
        _logger.LogWarning("Azure Service Bus not configured. Batch of {Count} events to topic {TopicName} was not published. " +
            "Configure 'AzureServiceBus:ConnectionString' to enable event publishing.", count, topicName);
        return Task.CompletedTask;
    }
}
