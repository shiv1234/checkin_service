using System.Diagnostics.CodeAnalysis;

namespace UnlockOKR.OKRSupoort.Persistence.EntityFrameworkDataAccess
{
    [ExcludeFromCodeCoverage]
    public class OkrSupportDbContext : IOkrTaskDbContext
    {
        public string Schema { get; }

        public string ConnectionString { get; }

        public OkrSupportDbContext(string connectionString, string schema)
        {
            ConnectionString = connectionString;
            Schema = schema;
        }
    }
}
