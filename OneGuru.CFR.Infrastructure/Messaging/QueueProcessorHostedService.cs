#nullable enable

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OneGuru.CFR.Domain.Common;
using OneGuru.CFR.Domain.Ports;

namespace OneGuru.CFR.Infrastructure.Messaging;

/// <summary>
/// Background hosted service for processing jobs from Azure Queue Storage.
/// </summary>
public class QueueProcessorHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QueueProcessorHostedService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(10);

    public QueueProcessorHostedService(
        IServiceProvider serviceProvider,
        ILogger<QueueProcessorHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Queue Processor Hosted Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessQueuesAsync(stoppingToken);
                await Task.Delay(_pollingInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Graceful shutdown
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing queues");
                try { await Task.Delay(_pollingInterval, stoppingToken); }
                catch (OperationCanceledException) { break; }
            }
        }

        _logger.LogInformation("Queue Processor Hosted Service is stopping.");
    }

    private async Task ProcessQueuesAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var backgroundJobQueue = scope.ServiceProvider.GetRequiredService<IBackgroundJobQueue>();

        // Process different queue types
        await ProcessNotificationQueueAsync(backgroundJobQueue, stoppingToken);
        await ProcessAuditLogQueueAsync(backgroundJobQueue, stoppingToken);
        await ProcessEmailQueueAsync(backgroundJobQueue, stoppingToken);
    }

    private async Task ProcessNotificationQueueAsync(IBackgroundJobQueue queue, CancellationToken stoppingToken)
    {
        try
        {
            var queueLength = await queue.GetQueueLengthAsync(AppConstants.QueueNotification, stoppingToken);
            if (queueLength == 0) return;

            _logger.LogDebug("Processing {Count} messages from notification queue", queueLength);

            // Process up to 10 messages per iteration
            for (int i = 0; i < Math.Min(queueLength, 10); i++)
            {
                var job = await queue.DequeueAsync<NotificationJob>(AppConstants.QueueNotification, stoppingToken);
                if (job == null) break;

                // Process the notification job
                _logger.LogInformation("Processing notification job: {JobType}", job.Type);
                // Add actual notification processing logic here
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing notification queue");
        }
    }

    private async Task ProcessAuditLogQueueAsync(IBackgroundJobQueue queue, CancellationToken stoppingToken)
    {
        try
        {
            var queueLength = await queue.GetQueueLengthAsync(AppConstants.QueueAuditLog, stoppingToken);
            if (queueLength == 0) return;

            _logger.LogDebug("Processing {Count} messages from audit log queue", queueLength);

            // Process up to 20 messages per iteration
            for (int i = 0; i < Math.Min(queueLength, 20); i++)
            {
                var job = await queue.DequeueAsync<AuditLogJob>(AppConstants.QueueAuditLog, stoppingToken);
                if (job == null) break;

                // Process the audit log job
                _logger.LogInformation("Processing audit log: {Action} on {EntityType}", job.Action, job.EntityType);
                // Add actual audit log persistence logic here
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing audit log queue");
        }
    }

    private async Task ProcessEmailQueueAsync(IBackgroundJobQueue queue, CancellationToken stoppingToken)
    {
        try
        {
            var queueLength = await queue.GetQueueLengthAsync(AppConstants.QueueEmail, stoppingToken);
            if (queueLength == 0) return;

            _logger.LogDebug("Processing {Count} messages from email queue", queueLength);

            // Process up to 5 messages per iteration
            for (int i = 0; i < Math.Min(queueLength, 5); i++)
            {
                var job = await queue.DequeueAsync<EmailJob>(AppConstants.QueueEmail, stoppingToken);
                if (job == null) break;

                // Process the email job
                _logger.LogInformation("Processing email job to: {Recipient}", job.To);
                // Add actual email sending logic here
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing email queue");
        }
    }
}

// Job DTOs for queue processing
public class NotificationJob
{
    public string Type { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string> Data { get; set; } = new();
}

public class AuditLogJob
{
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class EmailJob
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
}
