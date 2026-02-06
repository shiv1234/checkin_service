namespace OneGuru.CFR.Persistence.EntityFrameworkDataAccess
{
    /// <summary>
    /// Unit of Work interface for the per-organization OrgDbContext.
    /// Mirrors IUnitOfWorkAsync but bound to the org-specific database.
    /// </summary>
    public interface IOrgUnitOfWork : IDisposable
    {
        Task<IOperationStatus> SaveChangesAsync();
        IOperationStatus SaveChanges();
        IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : class;
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;
    }
}
