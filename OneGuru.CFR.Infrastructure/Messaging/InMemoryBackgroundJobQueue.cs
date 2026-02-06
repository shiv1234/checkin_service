#nullable enable

using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;
using OneGuru.CFR.Domain.Ports;

namespace OneGuru.CFR.Infrastructure.Messaging;

/// <summary>
/// In-memory implementation of IBackgroundJobQueue for development/testing when Azure Queue Storage is not configured.
/// </summary>
public class InMemoryBackgroundJobQueue : IBackgroundJobQueue
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<string>> _queues = new();
    private readonly ILogger<InMemoryBackgroundJobQueue> _logger;

    public InMemoryBackgroundJobQueue(ILogger<InMemoryBackgroundJobQueue> logger)
    {
        _logger = logger;
        _logger.LogWarning("Using in-memory background job queue. Configure 'AzureQueueStorage:ConnectionString' for production use.");
    }

    public Task EnqueueAsync<T>(string queueName, T jobPayload, TimeSpan? visibilityDelay = null, CancellationToken cancellationToken = default) where T : class
    {
        var queue = _queues.GetOrAdd(queueName, _ => new ConcurrentQueue<string>());
        var json = JsonSerializer.Serialize(jobPayload);
        queue.Enqueue(json);

        _logger.LogDebug("Enqueued job to in-memory queue {QueueName}", queueName);
        return Task.CompletedTask;
    }

    public Task<T?> DequeueAsync<T>(string queueName, CancellationToken cancellationToken = default) where T : class
    {
        if (_queues.TryGetValue(queueName, out var queue) && queue.TryDequeue(out var json))
        {
            var payload = JsonSerializer.Deserialize<T>(json);
            _logger.LogDebug("Dequeued job from in-memory queue {QueueName}", queueName);
            return Task.FromResult(payload);
        }

        return Task.FromResult<T?>(null);
    }

    public Task<int> GetQueueLengthAsync(string queueName, CancellationToken cancellationToken = default)
    {
        if (_queues.TryGetValue(queueName, out var queue))
        {
            return Task.FromResult(queue.Count);
        }
        return Task.FromResult(0);
    }

    public Task EnqueueBatchAsync<T>(string queueName, IEnumerable<T> jobs, CancellationToken cancellationToken = default) where T : class
    {
        var queue = _queues.GetOrAdd(queueName, _ => new ConcurrentQueue<string>());
        var count = 0;

        foreach (var job in jobs)
        {
            var json = JsonSerializer.Serialize(job);
            queue.Enqueue(json);
            count++;
        }

        _logger.LogDebug("Enqueued batch of {Count} jobs to in-memory queue {QueueName}", count, queueName);
        return Task.CompletedTask;
    }
}
