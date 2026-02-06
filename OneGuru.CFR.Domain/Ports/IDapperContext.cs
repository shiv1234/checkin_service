#nullable enable
namespace OneGuru.CFR.Domain.Ports
{
    /// <summary>
    /// Interface for Dapper-based raw SQL queries.
    /// Used selectively for performance-critical or complex queries
    /// that benefit from direct SQL over EF Core LINQ translation.
    /// </summary>
    public interface IDapperContext
    {
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null);
        Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null);
        Task<int> ExecuteAsync(string sql, object? param = null);
        Task<T> ExecuteScalarAsync<T>(string sql, object? param = null);
    }
}
