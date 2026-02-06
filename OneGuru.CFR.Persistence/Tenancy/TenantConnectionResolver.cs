#nullable enable
using Microsoft.Extensions.Configuration;
using OneGuru.CFR.Domain.Common;

namespace OneGuru.CFR.Persistence.Tenancy
{
    /// <summary>
    /// Centralizes tenant connection string resolution from Azure Key Vault configuration.
    /// Replaces duplicated resolution logic previously in Program.ConfigureDatabase and CfrContext.OnConfiguring.
    /// </summary>
    public static class TenantConnectionResolver
    {
        /// <summary>
        /// Decrypts an encrypted tenant ID from the request header.
        /// </summary>
        public static string? DecryptTenantId(string? encryptedTenantId)
        {
            if (string.IsNullOrEmpty(encryptedTenantId))
                return null;

            return Encryption.DecryptRijndael(encryptedTenantId, AppConstants.EncryptionPrivateKey);
        }

        /// <summary>
        /// Resolves the database connection string for a given tenant ID
        /// using Azure Key Vault configuration with retry policy.
        /// </summary>
        public static string? ResolveConnectionString(string tenantId, IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(tenantId))
                return null;

            var postFix = configuration.GetValue<string>("KeyVaultConfig:PostFix");
            var key = tenantId + "-Connection" + postFix;
            var connectionString = configuration.GetValue<string>(key);

            if (connectionString == null)
            {
                // Retry with config reload (Key Vault secrets may not be loaded yet)
                (configuration as IConfigurationRoot)?.Reload();
                connectionString = configuration.GetValue<string>(key);
            }

            return connectionString;
        }
    }
}
