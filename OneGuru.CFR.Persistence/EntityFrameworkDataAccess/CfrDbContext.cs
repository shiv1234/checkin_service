using System.Diagnostics.CodeAnalysis;

namespace OneGuru.CFR.Persistence.EntityFrameworkDataAccess
{
    [ExcludeFromCodeCoverage]
    public class CfrDbContext : ICfrDbContext
    {
        public string Schema { get; }

        public string ConnectionString { get; }

        public CfrDbContext(string connectionString, string schema)
        {
            ConnectionString = connectionString;
            Schema = schema;
        }
    }
}
