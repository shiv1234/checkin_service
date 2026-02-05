using UnlockOKR.OKRSupoort.Domain.ResponseModels;
using System.Threading.Tasks;

namespace UnlockOKR.OKRSupoort.Domain.Ports
{
    public interface IKeyVaultService
    {
        Task<BlobVaultResponse> GetAzureBlobKeysAsync();
        Task<ServiceSettingUrlResponse> GetSettingsAndUrlsAsync();
    }
}
