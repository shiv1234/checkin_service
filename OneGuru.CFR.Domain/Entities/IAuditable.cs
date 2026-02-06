namespace OneGuru.CFR.Domain.Entities
{
    /// <summary>
    /// Interface for entities that require audit tracking.
    /// The AuditSaveChangesInterceptor automatically populates these fields.
    /// </summary>
    public interface IAuditable
    {
        long CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
        long? UpdatedBy { get; set; }
        DateTime? UpdatedOn { get; set; }
    }
}
