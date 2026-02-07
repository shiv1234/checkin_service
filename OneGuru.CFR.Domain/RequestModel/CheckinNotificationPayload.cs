#nullable enable
namespace OneGuru.CFR.Domain.RequestModel
{
    public class CheckinSubmittedEvent
    {
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int WeekNumber { get; set; }
        public int Year { get; set; }
        public long CheckinId { get; set; }
        public string? NotificationMessage { get; set; }
    }

    public class LowPulseAlertEvent
    {
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int PulseScore { get; set; }
        public int WeekNumber { get; set; }
        public int Year { get; set; }
        public long CheckinId { get; set; }
        public string? NotificationMessage { get; set; }
    }

    public class ActiveCardRequest
    {
        public long EmployeeId { get; set; }
        public string Type { get; set; } = string.Empty;
        public long CheckinId { get; set; }
        public string Priority { get; set; } = "normal";
        public string? Description { get; set; }
    }
}
