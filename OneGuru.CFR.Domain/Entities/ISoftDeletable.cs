namespace OneGuru.CFR.Domain.Entities
{
    /// <summary>
    /// Marker interface for entities that support soft delete via IsActive flag.
    /// EF Core global query filters automatically exclude inactive records.
    /// </summary>
    public interface ISoftDeletable
    {
        bool IsActive { get; set; }
    }
}
