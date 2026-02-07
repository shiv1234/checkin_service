#nullable enable
using MediatR;
using Microsoft.Extensions.Logging;
using OneGuru.CFR.Domain.Commands;
using OneGuru.CFR.Domain.Common;
using OneGuru.CFR.Domain.Entities;
using OneGuru.CFR.Domain.Ports;
using OneGuru.CFR.Domain.RequestModel;
using OneGuru.CFR.Domain.ResponseModels;
using OneGuru.CFR.Persistence.EntityFrameworkDataAccess;

namespace OneGuru.CFR.Infrastructure.Handlers
{
    public class SubmitCheckinCommandHandler : IRequestHandler<SubmitCheckinCommand, Payload<SubmitCheckinResponse>>
    {
        private readonly IOrgUnitOfWork _unitOfWork;
        private readonly IAuditService _auditService;
        private readonly IBackgroundJobQueue _backgroundJobQueue;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<SubmitCheckinCommandHandler> _logger;

        public SubmitCheckinCommandHandler(
            IOrgUnitOfWork unitOfWork,
            IAuditService auditService,
            IBackgroundJobQueue backgroundJobQueue,
            IEventPublisher eventPublisher,
            ILogger<SubmitCheckinCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditService = auditService;
            _backgroundJobQueue = backgroundJobQueue;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<Payload<SubmitCheckinResponse>> Handle(
            SubmitCheckinCommand request,
            CancellationToken cancellationToken)
        {
            var currentUser = _auditService.GetCurrentUser();
            if (currentUser == null || currentUser.EmployeeId <= 0)
            {
                return ApiResult.Error<SubmitCheckinResponse>("Unable to identify current user.", 401);
            }

            // Enforce that employees can only submit their own check-ins
            request.EmployeeId = currentUser.EmployeeId;

            // Validate pulse score is present
            if (!request.PulseScore.HasValue)
            {
                return ApiResult.Error<SubmitCheckinResponse>("Pulse score is required before submission.");
            }

            var repo = _unitOfWork.RepositoryAsync<WeeklyCheckinResponse>();

            // Find existing check-in for this employee/week/year (upsert)
            var existing = await repo.FirstOrDefaultAsync(
                c => c.EmployeeId == request.EmployeeId
                     && c.WeekNumber == request.WeekNumber
                     && c.Year == request.Year);

            WeeklyCheckinResponse checkin;
            bool isUpdate = existing != null;

            if (isUpdate)
            {
                checkin = existing!;
                checkin.PulseScore = request.PulseScore;
                checkin.TaskCompletionRate = request.TaskCompletionRate;
                checkin.Wins = request.Wins;
                checkin.Blockers = request.Blockers;
                checkin.ManagerNote = request.ManagerNote;
                checkin.Status = CheckinStatus.Submitted;
                checkin.SubmittedAt = DateTime.UtcNow;
                repo.Update(checkin);
            }
            else
            {
                checkin = new WeeklyCheckinResponse
                {
                    EmployeeId = request.EmployeeId,
                    WeekNumber = request.WeekNumber,
                    Year = request.Year,
                    PulseScore = request.PulseScore,
                    TaskCompletionRate = request.TaskCompletionRate,
                    Wins = request.Wins,
                    Blockers = request.Blockers,
                    ManagerNote = request.ManagerNote,
                    Status = CheckinStatus.Submitted,
                    SubmittedAt = DateTime.UtcNow
                };
                await repo.AddAsync(checkin);
            }

            var saveResult = await _unitOfWork.SaveChangesAsync();
            if (!saveResult.Success)
            {
                _logger.LogError("Failed to save check-in for Employee {EmployeeId}, Week {Week}, Year {Year}",
                    request.EmployeeId, request.WeekNumber, request.Year);
                return ApiResult.Error<SubmitCheckinResponse>("Failed to save check-in. Please try again.");
            }

            // Audit log
            await _auditService.LogAuditAsync(
                isUpdate ? "UpdateCheckin" : "SubmitCheckin",
                "WeeklyCheckinResponse",
                checkin.Id.ToString(),
                cancellationToken: cancellationToken);

            var employeeName = $"{currentUser.FirstName} {currentUser.LastName}".Trim();

            // Send manager notification: "New check-in from {Employee Name}"
            await SendManagerNotificationAsync(checkin, employeeName, cancellationToken);

            // Create Active Card for manager review
            await CreateActiveCardAsync(checkin, "checkin_review", "normal", cancellationToken);

            // Low pulse alert if pulse_score <= 2
            if (request.PulseScore <= 2)
            {
                await SendLowPulseAlertAsync(checkin, employeeName, request.PulseScore.Value, cancellationToken);
                await CreateActiveCardAsync(checkin, "checkin_low_pulse", "urgent", cancellationToken);
            }

            var response = new SubmitCheckinResponse
            {
                CheckinId = checkin.Id,
                WeekNumber = checkin.WeekNumber,
                Year = checkin.Year,
                Status = checkin.Status,
                SubmittedAt = checkin.SubmittedAt!.Value,
                Message = $"Week {checkin.WeekNumber} locked in!"
            };

            return ApiResult.Success(response, $"Week {checkin.WeekNumber} locked in!");
        }

        private async Task SendManagerNotificationAsync(
            WeeklyCheckinResponse checkin, string employeeName, CancellationToken cancellationToken)
        {
            try
            {
                var notification = new CheckinSubmittedEvent
                {
                    EmployeeId = checkin.EmployeeId,
                    EmployeeName = employeeName,
                    WeekNumber = checkin.WeekNumber,
                    Year = checkin.Year,
                    CheckinId = checkin.Id,
                    NotificationMessage = $"New check-in from {employeeName}"
                };

                await _backgroundJobQueue.EnqueueAsync(
                    AppConstants.QueueNotification, notification, cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "Manager notification queued for check-in {CheckinId} from Employee {EmployeeId}",
                    checkin.Id, checkin.EmployeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send manager notification for check-in {CheckinId}", checkin.Id);
            }
        }

        private async Task SendLowPulseAlertAsync(
            WeeklyCheckinResponse checkin, string employeeName, int pulseScore, CancellationToken cancellationToken)
        {
            try
            {
                var alert = new LowPulseAlertEvent
                {
                    EmployeeId = checkin.EmployeeId,
                    EmployeeName = employeeName,
                    PulseScore = pulseScore,
                    WeekNumber = checkin.WeekNumber,
                    Year = checkin.Year,
                    CheckinId = checkin.Id,
                    NotificationMessage = $"{employeeName} submitted a low pulse check-in"
                };

                await _backgroundJobQueue.EnqueueAsync(
                    AppConstants.QueueNotification, alert, cancellationToken: cancellationToken);

                _logger.LogWarning(
                    "Low pulse alert queued: Employee {EmployeeId} scored {PulseScore} for Week {Week}/{Year}",
                    checkin.EmployeeId, pulseScore, checkin.WeekNumber, checkin.Year);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send low pulse alert for check-in {CheckinId}", checkin.Id);
            }
        }

        private async Task CreateActiveCardAsync(
            WeeklyCheckinResponse checkin, string cardType, string priority, CancellationToken cancellationToken)
        {
            try
            {
                var activeCard = new ActiveCardRequest
                {
                    EmployeeId = checkin.EmployeeId,
                    Type = cardType,
                    CheckinId = checkin.Id,
                    Priority = priority
                };

                await _backgroundJobQueue.EnqueueAsync(
                    AppConstants.QueueNotification, activeCard, cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "Active card '{CardType}' queued for check-in {CheckinId}",
                    cardType, checkin.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create active card for check-in {CheckinId}", checkin.Id);
            }
        }
    }
}
