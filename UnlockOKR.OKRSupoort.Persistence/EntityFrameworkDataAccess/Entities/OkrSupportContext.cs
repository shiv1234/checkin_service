using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UnlockOKR.OKRSupoort.Domain.Common;


#nullable disable

namespace UnlockOKR.OKRSupoort.Persistence.EntityFrameworkDataAccess.Entities
{
    [ExcludeFromCodeCoverage]
    public partial class OkrSupportContext : DbContext
    {
        private readonly HttpContext _httpContext;
        private readonly IConfiguration Configuration;
        private readonly ILogger<OkrSupportContext> _logger;

        public OkrSupportContext()
        {
        }

        public OkrSupportContext(DbContextOptions<OkrSupportContext> options, IConfiguration configuration, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor = null)
            : base(options)
        {
            _httpContext = httpContextAccessor?.HttpContext;
            Configuration = configuration;
            _logger = loggerFactory.CreateLogger<OkrSupportContext>();
        }

        public virtual DbSet<Dbversion> Dbversions { get; set; }
        public virtual DbSet<DbversionLog> DbversionLogs { get; set; }
        public virtual DbSet<ElasticPoolMaster> ElasticPoolMasters { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<FunctionDb> FunctionDbs { get; set; }
        public virtual DbSet<FunctionDbProd> FunctionDbProds { get; set; }
        public virtual DbSet<MigrationErrorLog> MigrationErrorLogs { get; set; }
        public virtual DbSet<MigrationLog> MigrationLogs { get; set; }
        public virtual DbSet<PurgeErrorLog> PurgeErrorLogs { get; set; }
        public virtual DbSet<PurgePolicy> PurgePolicies { get; set; }
        public virtual DbSet<TenantMaster> TenantMasters { get; set; }
        public virtual DbSet<TenantMasterProd> TenantMasterProds { get; set; }
        public virtual DbSet<TenantUserDetail> TenantUserDetails { get; set; }
        public virtual DbSet<TmpErrLog> TmpErrLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                
                var hasTenant = _httpContext.Request.Headers.TryGetValue("TenantId", out var tenantId);
                if (!hasTenant && _httpContext.Request.Host.Value.Contains("localhost"))
                    tenantId = Configuration.GetValue<string>("TenantId");

                if (hasTenant)
                    tenantId = Encryption.DecryptRijndael(tenantId, AppConstants.EncryptionPrivateKey);

                var defaultDbName = Configuration.GetValue<string>("ConnectionStrings:DBName");
                var connectionString = Configuration.GetValue<string>("ConnectionStrings:ConnectionString");
                foreach (string splitItem in connectionString.Split(";"))
                {
                    if (splitItem.Contains(defaultDbName))
                    {
                        int startIndex = splitItem.IndexOf(defaultDbName);
                        var replaceddefaultDbName = splitItem.Substring(startIndex);
                        var dbName = defaultDbName + "_" + tenantId;
                        connectionString = connectionString.Replace(replaceddefaultDbName, dbName);

                        break;
                    }
                }
                _logger.LogError("Connection String - " + connectionString);
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Dbversion>(entity =>
            {
                entity.Property(e => e.Dbname).IsUnicode(false);

                entity.Property(e => e.KeyVault).IsUnicode(false);
            });

            modelBuilder.Entity<DbversionLog>(entity =>
            {
                entity.Property(e => e.ErrorMessage).IsUnicode(false);
            });

            modelBuilder.Entity<ElasticPoolMaster>(entity =>
            {
                entity.Property(e => e.ElasticPoolName).IsUnicode(false);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ErrorLog>(entity =>
            {
                entity.HasKey(e => e.LogId)
                    .HasName("PK__ErrorLog__5E5486487E18CD02");
            });

            modelBuilder.Entity<FunctionDb>(entity =>
            {
                entity.Property(e => e.ConnectionServiceName).IsUnicode(false);

                entity.Property(e => e.Dbname).IsUnicode(false);

                entity.Property(e => e.ScriptName).IsUnicode(false);
            });

            modelBuilder.Entity<FunctionDbProd>(entity =>
            {
                entity.Property(e => e.ConnectionServiceName).IsUnicode(false);

                entity.Property(e => e.Dbname).IsUnicode(false);

                entity.Property(e => e.ScriptName).IsUnicode(false);
            });

            modelBuilder.Entity<MigrationErrorLog>(entity =>
            {
                entity.Property(e => e.Context).IsUnicode(false);

                entity.Property(e => e.FatherPid).IsUnicode(false);

                entity.Property(e => e.Job).IsUnicode(false);

                entity.Property(e => e.Message).IsUnicode(false);

                entity.Property(e => e.Origin).IsUnicode(false);

                entity.Property(e => e.Pid).IsUnicode(false);

                entity.Property(e => e.Project).IsUnicode(false);

                entity.Property(e => e.RootPid).IsUnicode(false);

                entity.Property(e => e.Type).IsUnicode(false);
            });

            modelBuilder.Entity<MigrationLog>(entity =>
            {
                entity.Property(e => e.SourceDbname).IsUnicode(false);

                entity.Property(e => e.TableName).IsUnicode(false);

                entity.Property(e => e.TargetDbname).IsUnicode(false);

                entity.Property(e => e.TenantId).IsUnicode(false);
            });

            modelBuilder.Entity<PurgeErrorLog>(entity =>
            {
                entity.Property(e => e.Context).IsUnicode(false);

                entity.Property(e => e.FatherPid).IsUnicode(false);

                entity.Property(e => e.Job).IsUnicode(false);

                entity.Property(e => e.Message).IsUnicode(false);

                entity.Property(e => e.Origin).IsUnicode(false);

                entity.Property(e => e.Pid).IsUnicode(false);

                entity.Property(e => e.Project).IsUnicode(false);

                entity.Property(e => e.RootPid).IsUnicode(false);

                entity.Property(e => e.Type).IsUnicode(false);
            });

            modelBuilder.Entity<PurgePolicy>(entity =>
            {
                entity.HasKey(e => e.PolicyId)
                    .HasName("PurgePolicy_pk");

                entity.Property(e => e.Dbname).IsUnicode(false);

                entity.Property(e => e.RefColumn).IsUnicode(false);

                entity.Property(e => e.RefTable).IsUnicode(false);
            });

            modelBuilder.Entity<TenantMaster>(entity =>
            {
                entity.HasKey(e => e.TenantId)
                    .HasName("PK_TenantMaster_TenantId");

                entity.Property(e => e.TenantId).ValueGeneratedNever();

                entity.Property(e => e.BufferLicense).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsBufferCreated).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsLicensed).HasDefaultValueSql("((0))");

                entity.Property(e => e.PurchaseLicense).HasDefaultValueSql("((0))");

                entity.Property(e => e.SubDomain).IsUnicode(false);
            });

            modelBuilder.Entity<TenantMasterProd>(entity =>
            {
                entity.Property(e => e.SubDomain).IsUnicode(false);
            });

            modelBuilder.Entity<TenantUserDetail>(entity =>
            {
                entity.HasKey(e => e.TenantUserId)
                    .HasName("PK_TenantUserDetails_TenantUserId");

                entity.Property(e => e.EmailId).IsUnicode(false);

                entity.HasOne(d => d.Tenant)
                    .WithMany(p => p.TenantUserDetails)
                    .HasForeignKey(d => d.TenantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TenantUserDetails_TenantId");
            });

            modelBuilder.Entity<TmpErrLog>(entity =>
            {
                entity.Property(e => e.Context).IsUnicode(false);

                entity.Property(e => e.FatherPid).IsUnicode(false);

                entity.Property(e => e.Job).IsUnicode(false);

                entity.Property(e => e.Message).IsUnicode(false);

                entity.Property(e => e.Origin).IsUnicode(false);

                entity.Property(e => e.Pid).IsUnicode(false);

                entity.Property(e => e.Project).IsUnicode(false);

                entity.Property(e => e.RootPid).IsUnicode(false);

                entity.Property(e => e.Type).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
