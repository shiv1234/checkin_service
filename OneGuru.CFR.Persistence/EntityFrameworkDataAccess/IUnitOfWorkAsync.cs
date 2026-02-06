using System.Threading.Tasks;

namespace OneGuru.CFR.Persistence.EntityFrameworkDataAccess
{
    public interface IUnitOfWorkAsync : IUnitOfWork
    {
        Task<IOperationStatus> SaveChangesAsync();
        IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : class;
    }
}
