#nullable enable

using OneGuru.CFR.Domain.RequestModel;

namespace OneGuru.CFR.Domain.Ports;

/// <summary>
/// Interface for audit logging and event emission.
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Logs an audit event for a domain action.
    /// </summary>
    /// <param name="action">The action being performed.</param>
    /// <param name="entityType">The type of entity being acted upon.</param>
    /// <param name="entityId">The ID of the entity.</param>
    /// <param name="oldValues">The previous values (for updates).</param>
    /// <param name="newValues">The new values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task LogAuditAsync(
        string action,
        string entityType,
        string entityId,
        object? oldValues = null,
        object? newValues = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs an audit event with user context.
    /// </summary>
    /// <param name="auditLog">The audit log request containing all details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task LogAuditAsync(AuditLogRequest auditLog, CancellationToken cancellationToken = default);

    /// <summary>
    /// Emits an integration event for cross-service communication.
    /// </summary>
    /// <typeparam name="T">The type of the event payload.</typeparam>
    /// <param name="eventName">The name of the event.</param>
    /// <param name="payload">The event payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task EmitIntegrationEventAsync<T>(string eventName, T payload, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Gets the current user identity from the context.
    /// </summary>
    UserIdentity? GetCurrentUser();

    /// <summary>
    /// Gets the current tenant ID from the context.
    /// </summary>
    string? GetCurrentTenantId();
}
