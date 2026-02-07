using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OneGuru.CFR.Domain.Common;

namespace OneGuru.CFR.Domain.Entities
{
    [Table("weekly_checkin_responses")]
    public class WeeklyCheckinResponse : BaseEntity
    {
        public long EmployeeId { get; set; }

        public int WeekNumber { get; set; }

        public int Year { get; set; }

        public int? PulseScore { get; set; }

        public int? TaskCompletionRate { get; set; }

        [MaxLength(4000)]
        public string Wins { get; set; }

        [MaxLength(4000)]
        public string Blockers { get; set; }

        [MaxLength(2000)]
        public string ManagerNote { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = CheckinStatus.Draft;

        public DateTime? SubmittedAt { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public long? ReviewedBy { get; set; }

        [MaxLength(1000)]
        public string ManagerComment { get; set; }
    }
}
