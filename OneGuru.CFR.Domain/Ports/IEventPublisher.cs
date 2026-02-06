#nullable enable

namespace OneGuru.CFR.Domain.Ports;

/// <summary>
/// Interface for publishing events to Azure Service Bus for event-driven communication.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes an event to a specific topic.
    /// </summary>
    /// <typeparam name="T">The type of the event payload.</typeparam>
    /// <param name="topicName">The name of the Service Bus topic.</param>
    /// <param name="eventPayload">The event data to publish.</param>
    /// <param name="correlationId">Optional correlation ID for tracing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PublishAsync<T>(string topicName, T eventPayload, string? correlationId = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Publishes an event to a specific queue.
    /// </summary>
    /// <typeparam name="T">The type of the event payload.</typeparam>
    /// <param name="queueName">The name of the Service Bus queue.</param>
    /// <param name="eventPayload">The event data to publish.</param>
    /// <param name="correlationId">Optional correlation ID for tracing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendToQueueAsync<T>(string queueName, T eventPayload, string? correlationId = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Publishes a batch of events to a specific topic.
    /// </summary>
    /// <typeparam name="T">The type of the event payloads.</typeparam>
    /// <param name="topicName">The name of the Service Bus topic.</param>
    /// <param name="events">The collection of events to publish.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PublishBatchAsync<T>(string topicName, IEnumerable<T> events, CancellationToken cancellationToken = default) where T : class;
}
