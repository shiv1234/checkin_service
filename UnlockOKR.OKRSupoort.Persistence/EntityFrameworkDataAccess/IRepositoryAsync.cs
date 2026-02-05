using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace UnlockOKR.OKRSupoort.Persistence.EntityFrameworkDataAccess
{
    public interface IRepositoryAsync<T> : IRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task<bool> AddRangeAsync(IEnumerable<T> entities);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> FindOneAsync(Expression<Func<T, bool>> predicate);
        Task<T> FindAsync(params object[] keyValues);
        Task<T> FindAsync(CancellationToken ct, params object[] keyValues);
        Task<IQueryable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<bool> DeleteAsync(params object[] keyValues);
        Task<bool> DeleteAsync(CancellationToken ct, params object[] keyValues);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    }
}
