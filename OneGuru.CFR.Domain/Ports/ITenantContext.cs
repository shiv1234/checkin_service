#nullable enable
namespace OneGuru.CFR.Domain.Ports
{
    /// <summary>
    /// Provides the resolved tenant context for the current request.
    /// Centralizes tenant resolution logic previously duplicated across CfrContext and Program.cs.
    /// </summary>
    public interface ITenantContext
    {
        /// <summary>
        /// The decrypted tenant identifier for the current request.
        /// </summary>
        string? TenantId { get; }

        /// <summary>
        /// The resolved database connection string for the current tenant.
        /// </summary>
        string? ConnectionString { get; }

        /// <summary>
        /// Whether the tenant context has been successfully resolved.
        /// </summary>
        bool IsResolved { get; }
    }
}
