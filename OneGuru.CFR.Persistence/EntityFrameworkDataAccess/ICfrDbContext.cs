namespace OneGuru.CFR.Persistence.EntityFrameworkDataAccess
{
    public interface ICfrDbContext
    {
        string ConnectionString { get; }
        string Schema { get; }
    }
}
