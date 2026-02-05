namespace UnlockOKR.OKRSupoort.Persistence.EntityFrameworkDataAccess
{
    public interface IDbContextSchema
    {
        string ConnectionString { get; }
        string Schema { get; }
    }
}
