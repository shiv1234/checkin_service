using System.Diagnostics.CodeAnalysis;

namespace UnlockOKR.OKRSupoort.Persistence.EntityFrameworkDataAccess
{
    [ExcludeFromCodeCoverage]
    public class DbContextSchema : IDbContextSchema
    {
        public string Schema { get; }

        public string ConnectionString { get; }

        public DbContextSchema(string connectionString, string schema)
        {
            ConnectionString = connectionString;
            Schema = schema;
        }
    }
}
