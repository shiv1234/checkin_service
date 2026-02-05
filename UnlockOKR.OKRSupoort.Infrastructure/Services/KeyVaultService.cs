using Microsoft.Extensions.Configuration;
using UnlockOKR.OKRSupoort.Domain.Common;
using UnlockOKR.OKRSupoort.Domain.Ports;
using UnlockOKR.OKRSupoort.Domain.ResponseModels;
using UnlockOKR.OKRSupoort.Infrastructure.Services.Contracts;
using System.Threading.Tasks;

namespace UnlockOKR.OKRSupoort.Infrastructure.Services
{
    public class KeyVaultService : BaseService, IKeyVaultService
    {
        public ICommonService CommonService { get; set; }
        public ISystemService SystemService { get; set; }
        
        [System.Obsolete]
        public KeyVaultService(IServicesAggregator servicesAggregateService,
            ICommonService commonService, ISystemService systemService) : base(servicesAggregateService)
        {
            CommonService = commonService;
            SystemService = systemService;
        }
        public async Task<BlobVaultResponse> GetAzureBlobKeysAsync()
        {
            if (!CommonService.IsTokenActive) return null;
            BlobVaultResponse blobVaultResponse = new BlobVaultResponse();
            var hasTenant = SystemService.HttpContext.Request.Headers.TryGetValue("TenantId", out var tenantId);
            if ((!hasTenant && SystemService.HttpContext.Request.Host.Value.Contains("localhost")))
                tenantId = Configuration.GetValue<string>("TenantId");

            if (!string.IsNullOrEmpty(tenantId))
            {
                var tenantString = Encryption.DecryptRijndael(tenantId, AppConstants.EncryptionPrivateKey);
                blobVaultResponse.BlobAccountKey = Configuration.GetValue<string>("AzureBlob:BlobAccountKey");
                blobVaultResponse.BlobAccountName = Configuration.GetValue<string>("AzureBlob:BlobAccountName");
                blobVaultResponse.BlobContainerName = tenantString;
                blobVaultResponse.BlobCdnUrl = Configuration.GetValue<string>("AzureBlob:BlobCdnUrl");
                blobVaultResponse.BlobCdnCommonUrl = Configuration.GetValue<string>("AzureBlob:BlobCdnUrl") + "common/";

            }

            return await Task.FromResult(blobVaultResponse);
        }
        public async Task<ServiceSettingUrlResponse> GetSettingsAndUrlsAsync()
        {
            if (!CommonService.IsTokenActive) return null;
            string domain;
            var hasOrigin = SystemService.HttpContext.Request.Headers.TryGetValue("OriginHost", out var origin);
            if (!hasOrigin && SystemService.HttpContext.Request.Host.Value.Contains("localhost"))
                domain = Configuration.GetValue<string>("FrontEndUrl").ToString();
            else
                domain = string.IsNullOrEmpty(origin) ? string.Empty : origin.ToString();
            ServiceSettingUrlResponse settingsResponse = new ServiceSettingUrlResponse
            {
                UnlockLog = Configuration.GetValue<string>("OkrService:UnlockLog"),
                OkrBaseAddress = Configuration.GetValue<string>("OkrService:BaseUrl"),
                OkrUnlockTime = Configuration.GetValue<string>("OkrService:UnlockTime"),
                FrontEndUrl = domain,
                ResetPassUrl = Configuration.GetValue<string>("ResetPassUrl"),
                NotificationBaseAddress = Configuration.GetValue<string>("Notification:BaseUrl"),
                TenantBaseAddress = Configuration.GetValue<string>("TenantService:BaseUrl")
            };
            return await Task.FromResult(settingsResponse);
        }
    }
}
