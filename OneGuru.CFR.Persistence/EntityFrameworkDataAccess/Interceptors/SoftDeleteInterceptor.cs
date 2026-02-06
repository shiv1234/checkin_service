#nullable enable
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OneGuru.CFR.Domain.Entities;

namespace OneGuru.CFR.Persistence.EntityFrameworkDataAccess.Interceptors
{
    /// <summary>
    /// EF Core interceptor that converts hard deletes to soft deletes for ISoftDeletable entities.
    /// Sets IsActive = false and changes entity state from Deleted to Modified.
    /// </summary>
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData, InterceptionResult<int> result)
        {
            ConvertToSoftDelete(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ConvertToSoftDelete(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void ConvertToSoftDelete(DbContext? context)
        {
            if (context == null) return;

            foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsActive = false;
                }
            }
        }
    }
}
