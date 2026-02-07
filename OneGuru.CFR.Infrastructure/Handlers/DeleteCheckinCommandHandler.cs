#nullable enable
using MediatR;
using Microsoft.Extensions.Logging;
using OneGuru.CFR.Domain.Commands;
using OneGuru.CFR.Domain.Entities;
using OneGuru.CFR.Domain.Ports;
using OneGuru.CFR.Domain.ResponseModels;
using OneGuru.CFR.Persistence.EntityFrameworkDataAccess;

namespace OneGuru.CFR.Infrastructure.Handlers
{
    public class DeleteCheckinCommandHandler : IRequestHandler<DeleteCheckinCommand, Payload<bool>>
    {
        private readonly IOrgUnitOfWork _unitOfWork;
        private readonly IAuditService _auditService;
        private readonly ILogger<DeleteCheckinCommandHandler> _logger;

        public DeleteCheckinCommandHandler(
            IOrgUnitOfWork unitOfWork,
            IAuditService auditService,
            ILogger<DeleteCheckinCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<Payload<bool>> Handle(
            DeleteCheckinCommand request,
            CancellationToken cancellationToken)
        {
            var currentUser = _auditService.GetCurrentUser();
            if (currentUser == null || currentUser.EmployeeId <= 0)
            {
                return ApiResult.Error<bool>("Unable to identify current user.", 401);
            }

            var repo = _unitOfWork.RepositoryAsync<WeeklyCheckinResponse>();
            var checkin = await repo.FirstOrDefaultAsync(
                c => c.Id == request.CheckinId);

            if (checkin == null)
            {
                return ApiResult.NotFound<bool>("Check-in not found.");
            }

            // Only the owner can delete their own check-in
            if (checkin.EmployeeId != currentUser.EmployeeId)
            {
                return ApiResult.Forbidden<bool>("You can only delete your own check-ins.");
            }

            repo.Delete(checkin);
            var saveResult = await _unitOfWork.SaveChangesAsync();

            if (!saveResult.Success)
            {
                _logger.LogError("Failed to delete check-in {CheckinId} for Employee {EmployeeId}",
                    request.CheckinId, currentUser.EmployeeId);
                return ApiResult.Error<bool>("Failed to delete check-in. Please try again.");
            }

            await _auditService.LogAuditAsync(
                "DeleteCheckin",
                "WeeklyCheckinResponse",
                checkin.Id.ToString(),
                cancellationToken: cancellationToken);

            _logger.LogInformation("Check-in {CheckinId} deleted by Employee {EmployeeId}",
                request.CheckinId, currentUser.EmployeeId);

            return ApiResult.Success(true, "Check-in deleted successfully.");
        }
    }
}
