namespace UnlockOKR.OKRSupoort.Persistence.EntityFrameworkDataAccess
{
    public interface IOkrTaskDbContext
    {
        string ConnectionString { get; }
        string Schema { get; }
    }
}
