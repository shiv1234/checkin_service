#nullable enable

namespace OneGuru.CFR.Domain.Ports;

/// <summary>
/// Interface for queueing background jobs to Azure Queue Storage.
/// </summary>
public interface IBackgroundJobQueue
{
    /// <summary>
    /// Enqueues a background job for processing.
    /// </summary>
    /// <typeparam name="T">The type of the job payload.</typeparam>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="jobPayload">The job data to enqueue.</param>
    /// <param name="visibilityDelay">Optional delay before the message becomes visible.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task EnqueueAsync<T>(string queueName, T jobPayload, TimeSpan? visibilityDelay = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Dequeues and processes the next job from the queue.
    /// </summary>
    /// <typeparam name="T">The type of the job payload.</typeparam>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The dequeued job payload, or null if the queue is empty.</returns>
    Task<T?> DequeueAsync<T>(string queueName, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Gets the approximate number of messages in the queue.
    /// </summary>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<int> GetQueueLengthAsync(string queueName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enqueues multiple jobs in a batch.
    /// </summary>
    /// <typeparam name="T">The type of the job payloads.</typeparam>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="jobs">The collection of jobs to enqueue.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task EnqueueBatchAsync<T>(string queueName, IEnumerable<T> jobs, CancellationToken cancellationToken = default) where T : class;
}
