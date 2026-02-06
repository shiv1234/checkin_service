#nullable enable

using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using OneGuru.CFR.Domain.Ports;

namespace OneGuru.CFR.Infrastructure.Messaging;

/// <summary>
/// Azure Queue Storage implementation of IBackgroundJobQueue for background processing.
/// </summary>
public class AzureQueueBackgroundProcessor : IBackgroundJobQueue
{
    private readonly string _connectionString;
    private readonly ILogger<AzureQueueBackgroundProcessor> _logger;
    private readonly Dictionary<string, QueueClient> _queueClients = new();
    private readonly SemaphoreSlim _clientLock = new(1, 1);

    public AzureQueueBackgroundProcessor(string connectionString, ILogger<AzureQueueBackgroundProcessor> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public async Task EnqueueAsync<T>(string queueName, T jobPayload, TimeSpan? visibilityDelay = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var client = await GetOrCreateQueueClientAsync(queueName, cancellationToken);
            var json = JsonSerializer.Serialize(jobPayload);
            var base64Message = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));

            await client.SendMessageAsync(base64Message, visibilityDelay ?? TimeSpan.Zero, null, cancellationToken);

            _logger.LogInformation("Enqueued job to queue {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue job to queue {QueueName}", queueName);
            throw;
        }
    }

    public async Task<T?> DequeueAsync<T>(string queueName, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var client = await GetOrCreateQueueClientAsync(queueName, cancellationToken);
            var response = await client.ReceiveMessageAsync(TimeSpan.FromMinutes(5), cancellationToken);

            if (response?.Value == null)
            {
                return null;
            }

            var message = response.Value;
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(message.MessageText));
            var payload = JsonSerializer.Deserialize<T>(json);

            // Delete the message after successful processing
            await client.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);

            _logger.LogInformation("Dequeued and processed job from queue {QueueName}", queueName);
            return payload;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dequeue job from queue {QueueName}", queueName);
            throw;
        }
    }

    public async Task<int> GetQueueLengthAsync(string queueName, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = await GetOrCreateQueueClientAsync(queueName, cancellationToken);
            var properties = await client.GetPropertiesAsync(cancellationToken);
            return properties.Value.ApproximateMessagesCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get queue length for {QueueName}", queueName);
            throw;
        }
    }

    public async Task EnqueueBatchAsync<T>(string queueName, IEnumerable<T> jobs, CancellationToken cancellationToken = default) where T : class
    {
        var client = await GetOrCreateQueueClientAsync(queueName, cancellationToken);
        var jobList = jobs.ToList();

        foreach (var job in jobList)
        {
            var json = JsonSerializer.Serialize(job);
            var base64Message = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
            await client.SendMessageAsync(base64Message, cancellationToken: cancellationToken);
        }

        _logger.LogInformation("Enqueued batch of {Count} jobs to queue {QueueName}", jobList.Count, queueName);
    }

    private async Task<QueueClient> GetOrCreateQueueClientAsync(string queueName, CancellationToken cancellationToken)
    {
        await _clientLock.WaitAsync(cancellationToken);
        try
        {
            if (!_queueClients.TryGetValue(queueName, out var client))
            {
                client = new QueueClient(_connectionString, queueName.ToLowerInvariant());
                await client.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
                _queueClients[queueName] = client;
            }
            return client;
        }
        finally
        {
            _clientLock.Release();
        }
    }
}
