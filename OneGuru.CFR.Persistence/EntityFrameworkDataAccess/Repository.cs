using Microsoft.EntityFrameworkCore;
using OneGuru.CFR.Persistence.EntityFrameworkDataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace OneGuru.CFR.Persistence.EntityFrameworkDataAccess
{
    [ExcludeFromCodeCoverage]
    public class Repository<T> : IRepositoryAsync<T> where T : class
    {
        protected readonly CfrContext Context;
        private readonly DbSet<T> _dbSet;
        public Repository(CfrContext context)
        {
            this.Context = context;
            _dbSet = context.Set<T>();
        }
        public void Add(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            _dbSet.Add(entity);
        }

        public async Task<T> AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<bool> AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null) throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            await _dbSet.AddRangeAsync(entities);
            return true;
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync();
        }

        public void Delete(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            _dbSet.Remove(entity);
        }

        public async Task<bool> DeleteAsync(params object[] keyValues)
        {
            var entity = await FindAsync(keyValues);
            if (entity == null)
                return false;
            _dbSet.Remove(entity);
            return true;
        }

        public async Task<bool> DeleteAsync(CancellationToken ct, params object[] keyValues)
        {
            var entity = await FindAsync(ct, keyValues);
            if (entity == null)
                return false;
            _dbSet.Remove(entity);
            return true;
        }

        public async Task<T> FindAsync(params object[] keyValues)
        {
            return await _dbSet.FindAsync(keyValues);
        }

        public async Task<T> FindAsync(CancellationToken ct, params object[] keyValues)
        {
            return await _dbSet.FindAsync(keyValues, ct);
        }

        public async Task<IQueryable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() => _dbSet.Where(predicate));
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public T FindOne(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.SingleOrDefault(predicate);
        }

        public async Task<T> FindOneAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.SingleOrDefaultAsync(predicate);
        }
        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public int GetCount()
        {
            return _dbSet.Count();
        }

        public IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public void Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            _dbSet.Update(entity);
        }
    }
}
