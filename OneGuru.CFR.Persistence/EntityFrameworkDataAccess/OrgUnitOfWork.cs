using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace OneGuru.CFR.Persistence.EntityFrameworkDataAccess
{
    /// <summary>
    /// Unit of Work implementation for the per-organization OrgDbContext.
    /// Mirrors the existing UnitOfWork pattern but operates on org-specific data.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class OrgUnitOfWork : IOrgUnitOfWork
    {
        private Dictionary<string, dynamic> _repositories;
        private bool _disposed;
        public OrgDbContext Context { get; private set; }

        public OrgUnitOfWork(OrgDbContext context)
        {
            Context = context;
            _repositories = new Dictionary<string, dynamic>();
        }

        public async Task<IOperationStatus> SaveChangesAsync()
        {
            IOperationStatus opStatus = new OperationStatus { Success = false };
            try
            {
                int numRec = await Context.SaveChangesAsync();
                opStatus.Success = true;
                opStatus.Message = "Record successfully saved!";
                opStatus.RecordsAffected = numRec;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var message = "The object is already modified. Please refresh the container before updating the object again.";
                opStatus = opStatus.CreateFromException(ex);
                opStatus.Message = message;
            }
            catch (SystemException ex)
            {
                opStatus = opStatus.CreateFromException(ex);
                if (!string.IsNullOrWhiteSpace(opStatus.InnerInnerMessage))
                    opStatus.Message = opStatus.Message + $"{Environment.NewLine} Exception: {opStatus.InnerInnerMessage}";
            }
            return opStatus;
        }

        public IOperationStatus SaveChanges()
        {
            IOperationStatus opStatus = new OperationStatus { Success = false };
            try
            {
                int numRec = Context.SaveChanges();
                opStatus.Success = true;
                opStatus.RecordsAffected = numRec;
            }
            catch (SystemException ex)
            {
                opStatus = opStatus.CreateFromException(ex);
            }
            return opStatus;
        }

        public IRepositoryAsync<T> RepositoryAsync<T>() where T : class
        {
            if (_repositories == null)
                _repositories = new Dictionary<string, dynamic>();

            var type = typeof(T).Name;

            if (_repositories.ContainsKey(type))
                return (IRepositoryAsync<T>)_repositories[type];

            var repositoryType = typeof(Repository<>);
            _repositories.Add(type, Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), Context));

            return _repositories[type];
        }

        public IRepository<T> Repository<T>() where T : class
        {
            return RepositoryAsync<T>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing && Context != null)
            {
                Context.Dispose();
                Context = null;
            }
            _disposed = true;
        }
    }
}
