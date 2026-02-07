#nullable enable
using MediatR;
using OneGuru.CFR.Domain.ResponseModels;

namespace OneGuru.CFR.Domain.Commands
{
    public class SubmitCheckinCommand : IRequest<Payload<SubmitCheckinResponse>>
    {
        public long EmployeeId { get; set; }
        public int WeekNumber { get; set; }
        public int Year { get; set; }
        public int? PulseScore { get; set; }
        public int? TaskCompletionRate { get; set; }
        public string? Wins { get; set; }
        public string? Blockers { get; set; }
        public string? ManagerNote { get; set; }
    }
}
