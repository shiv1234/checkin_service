using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OneGuru.CFR.Domain.Entities;

namespace OneGuru.CFR.Persistence.EntityFrameworkDataAccess
{
    /// <summary>
    /// DbContext for per-organization operational data (check-ins, pulse, tasks, etc.).
    /// Uses tenant-resolved connection string and applies global soft delete query filters.
    /// Feature-specific work orders will add DbSets to this context.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class OrgDbContext : DbContext
    {
        public OrgDbContext(DbContextOptions<OrgDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            // Apply global soft delete query filter to all ISoftDeletable entities
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(OrgDbContext)
                        .GetMethod(nameof(ApplySoftDeleteFilter),
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                        .MakeGenericMethod(entityType.ClrType);

                    method.Invoke(null, new object[] { modelBuilder });
                }
            }

            OnModelCreatingPartial(modelBuilder);
        }

        private static void ApplySoftDeleteFilter<T>(ModelBuilder modelBuilder) where T : class, ISoftDeletable
        {
            modelBuilder.Entity<T>().HasQueryFilter(e => e.IsActive);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
