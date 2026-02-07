#nullable enable
namespace OneGuru.CFR.Domain.ResponseModels
{
    public class SubmitCheckinResponse
    {
        public long CheckinId { get; set; }
        public int WeekNumber { get; set; }
        public int Year { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
