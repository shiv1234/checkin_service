using Microsoft.EntityFrameworkCore;
using OneGuru.CFR.Domain.Entities;

namespace OneGuru.CFR.Persistence.EntityFrameworkDataAccess
{
    public partial class OrgDbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeeklyCheckinResponse>(entity =>
            {
                entity.HasIndex(e => new { e.EmployeeId, e.WeekNumber, e.Year })
                    .IsUnique()
                    .HasDatabaseName("IX_WeeklyCheckinResponses_Employee_Week_Year");

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValue("DRAFT");
            });
        }
    }
}
