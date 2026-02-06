using OneGuru.CFR.Domain.ResponseModels;
using System.Threading.Tasks;

namespace OneGuru.CFR.Domain.Ports
{
    public interface IKeyVaultService
    {
        Task<BlobVaultResponse> GetAzureBlobKeysAsync();
        Task<ServiceSettingUrlResponse> GetSettingsAndUrlsAsync();
    }
}
