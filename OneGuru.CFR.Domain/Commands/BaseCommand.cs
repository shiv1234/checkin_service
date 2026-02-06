using System;

namespace OneGuru.CFR.Domain.Commands
{
    public class BaseCommand
    {
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
