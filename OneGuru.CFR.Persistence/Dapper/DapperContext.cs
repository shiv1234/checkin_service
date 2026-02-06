#nullable enable
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using OneGuru.CFR.Domain.Ports;

namespace OneGuru.CFR.Persistence.Dapper
{
    /// <summary>
    /// Dapper-based data access for performance-critical or complex queries.
    /// Uses the tenant-resolved connection string from ITenantContext.
    /// </summary>
    public class DapperContext : IDapperContext
    {
        private readonly ITenantContext _tenantContext;

        public DapperContext(ITenantContext tenantContext)
        {
            _tenantContext = tenantContext;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<T>(sql, param);
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null)
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(sql, param);
        }

        public async Task<int> ExecuteAsync(string sql, object? param = null)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteAsync(sql, param);
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, object? param = null)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<T>(sql, param) ?? default!;
        }

        private IDbConnection CreateConnection()
        {
            if (!_tenantContext.IsResolved || string.IsNullOrEmpty(_tenantContext.ConnectionString))
                throw new InvalidOperationException("Tenant context is not resolved. Cannot create database connection.");

            return new SqlConnection(_tenantContext.ConnectionString);
        }
    }
}
