#nullable enable
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OneGuru.CFR.Domain.Entities;

namespace OneGuru.CFR.Persistence.EntityFrameworkDataAccess.Interceptors
{
    /// <summary>
    /// EF Core interceptor that automatically populates audit fields (CreatedBy/On, UpdatedBy/On)
    /// on IAuditable entities during SaveChanges.
    /// </summary>
    public class AuditSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditSaveChangesInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData, InterceptionResult<int> result)
        {
            ApplyAuditFields(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ApplyAuditFields(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ApplyAuditFields(DbContext? context)
        {
            if (context == null) return;

            var userId = GetCurrentUserId();
            var now = DateTime.UtcNow;

            foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = userId;
                        entry.Entity.CreatedOn = now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedBy = userId;
                        entry.Entity.UpdatedOn = now;
                        // Prevent overwriting original creation audit
                        entry.Property(nameof(IAuditable.CreatedBy)).IsModified = false;
                        entry.Property(nameof(IAuditable.CreatedOn)).IsModified = false;
                        break;
                }
            }
        }

        private long GetCurrentUserId()
        {
            var httpContext = _httpContextAccessor?.HttpContext;
            if (httpContext == null) return 0;

            var employeeIdClaim = httpContext.User?.FindFirst("employeeId")?.Value;
            if (long.TryParse(employeeIdClaim, out var employeeId))
                return employeeId;

            return 0;
        }
    }
}
