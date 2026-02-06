#nullable enable

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text.Json;
using OneGuru.CFR.Domain.Common;
using OneGuru.CFR.Domain.Ports;
using OneGuru.CFR.Domain.RequestModel;
using OneGuru.CFR.Infrastructure.Messaging;

namespace OneGuru.CFR.Infrastructure.Services;

/// <summary>
/// Service for audit logging and integration event emission.
/// </summary>
public class AuditService : IAuditService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBackgroundJobQueue _backgroundJobQueue;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<AuditService> _logger;

    public AuditService(
        IHttpContextAccessor httpContextAccessor,
        IBackgroundJobQueue backgroundJobQueue,
        IEventPublisher eventPublisher,
        ILogger<AuditService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _backgroundJobQueue = backgroundJobQueue;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task LogAuditAsync(
        string action,
        string entityType,
        string entityId,
        object? oldValues = null,
        object? newValues = null,
        CancellationToken cancellationToken = default)
    {
        var user = GetCurrentUser();
        var tenantId = GetCurrentTenantId();

        var auditJob = new AuditLogJob
        {
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            UserId = user?.EmployeeId.ToString() ?? "anonymous",
            TenantId = tenantId ?? "unknown",
            OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
            Timestamp = DateTime.UtcNow
        };

        try
        {
            // Queue the audit log for async processing
            await _backgroundJobQueue.EnqueueAsync(AppConstants.QueueAuditLog, auditJob, cancellationToken: cancellationToken);

            _logger.LogDebug("Queued audit log: {Action} on {EntityType} {EntityId}",
                action, entityType, entityId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue audit log for {Action} on {EntityType} {EntityId}",
                action, entityType, entityId);
        }
    }

    public async Task LogAuditAsync(AuditLogRequest auditLog, CancellationToken cancellationToken = default)
    {
        await LogAuditAsync(
            auditLog.ActionType ?? "Unknown",
            auditLog.EntityType ?? "Unknown",
            auditLog.EntityId?.ToString() ?? "0",
            null,
            auditLog,
            cancellationToken);
    }

    public async Task EmitIntegrationEventAsync<T>(string eventName, T payload, CancellationToken cancellationToken = default) where T : class
    {
        var correlationId = GetCorrelationId();

        try
        {
            await _eventPublisher.PublishAsync(eventName, payload, correlationId, cancellationToken);

            _logger.LogInformation("Emitted integration event {EventName} with correlation ID {CorrelationId}",
                eventName, correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to emit integration event {EventName}", eventName);
            throw;
        }
    }

    public UserIdentity? GetCurrentUser()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var claims = httpContext.User.Claims;

        var userIdentity = new UserIdentity
        {
            EmailId = claims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty,
            FirstName = claims.FirstOrDefault(c => c.Type == "name")?.Value?.Split(' ').FirstOrDefault() ?? string.Empty,
            LastName = claims.FirstOrDefault(c => c.Type == "name")?.Value?.Split(' ').Skip(1).FirstOrDefault() ?? string.Empty
        };

        if (long.TryParse(claims.FirstOrDefault(c => c.Type == "employeeId")?.Value, out var employeeId))
        {
            userIdentity.EmployeeId = employeeId;
        }

        var isImpersonateClaim = claims.FirstOrDefault(c => c.Type == "isImpersonateUser");
        if (isImpersonateClaim != null && bool.TryParse(isImpersonateClaim.Value, out var isImpersonate))
        {
            userIdentity.IsImpersonatedUser = isImpersonate;
            userIdentity.ImpersonatedBy = claims.FirstOrDefault(c => c.Type == "impersonateBy")?.Value ?? string.Empty;

            if (long.TryParse(claims.FirstOrDefault(c => c.Type == "impersonateById")?.Value, out var impersonateById))
            {
                userIdentity.ImpersonatedById = impersonateById;
            }
        }

        return userIdentity;
    }

    public string? GetCurrentTenantId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return null;
        }

        // Try to get from claims first
        var tenantClaim = httpContext.User?.Claims.FirstOrDefault(c => c.Type == "tenantId");
        if (tenantClaim != null)
        {
            return tenantClaim.Value;
        }

        // Fall back to header
        if (httpContext.Request.Headers.TryGetValue("TenantId", out var tenantId))
        {
            return Encryption.DecryptRijndael(tenantId!, AppConstants.EncryptionPrivateKey);
        }

        return null;
    }

    private string GetCorrelationId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId) == true)
        {
            return correlationId!;
        }
        return Guid.NewGuid().ToString();
    }
}
