using System;

namespace OneGuru.CFR.Persistence.EntityFrameworkDataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        IOperationStatus SaveChanges();
        bool Commit();
        void Rollback();
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;
    }
}
