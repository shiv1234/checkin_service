#nullable enable
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OneGuru.CFR.Domain.Ports;
using OneGuru.CFR.Persistence.Tenancy;

namespace OneGuru.CFR.Infrastructure.Tenancy
{
    /// <summary>
    /// Resolves tenant context from the current HTTP request.
    /// Extracts encrypted TenantId from headers or claims, decrypts it,
    /// and resolves the per-tenant database connection string.
    /// Scoped lifetime - cached per request.
    /// </summary>
    public class HttpTenantContext : ITenantContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private bool _initialized;
        private string? _tenantId;
        private string? _connectionString;

        public HttpTenantContext(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public string? TenantId
        {
            get
            {
                EnsureInitialized();
                return _tenantId;
            }
        }

        public string? ConnectionString
        {
            get
            {
                EnsureInitialized();
                return _connectionString;
            }
        }

        public bool IsResolved
        {
            get
            {
                EnsureInitialized();
                return !string.IsNullOrEmpty(_tenantId) && !string.IsNullOrEmpty(_connectionString);
            }
        }

        private void EnsureInitialized()
        {
            if (_initialized) return;
            _initialized = true;

            var httpContext = _httpContextAccessor?.HttpContext;
            if (httpContext == null) return;

            // Try claims first (set by TokenManagerMiddleware)
            _tenantId = httpContext.User?.FindFirst("tenantId")?.Value;

            // Fallback: decrypt from TenantId header
            if (string.IsNullOrEmpty(_tenantId))
            {
                var hasTenant = httpContext.Request.Headers.TryGetValue("TenantId", out var encryptedTenantId);
                if (hasTenant && !string.IsNullOrEmpty(encryptedTenantId.ToString()))
                {
                    _tenantId = TenantConnectionResolver.DecryptTenantId(encryptedTenantId.ToString());
                }
            }

            // Fallback: localhost development
            if (string.IsNullOrEmpty(_tenantId) && httpContext.Request.Host.Value?.Contains("localhost") == true)
            {
                _tenantId = _configuration.GetValue<string>("TenantId");
            }

            // Resolve connection string
            if (!string.IsNullOrEmpty(_tenantId))
            {
                _connectionString = TenantConnectionResolver.ResolveConnectionString(_tenantId, _configuration);
            }
        }
    }
}
