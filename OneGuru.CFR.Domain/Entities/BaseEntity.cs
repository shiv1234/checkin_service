namespace OneGuru.CFR.Domain.Entities
{
    /// <summary>
    /// Abstract base class for all per-organization operational entities.
    /// Provides identity, soft delete, and audit fields consistent with existing patterns.
    /// </summary>
    public abstract class BaseEntity : IAuditable, ISoftDeletable
    {
        public long Id { get; set; }
        public bool IsActive { get; set; } = true;
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
