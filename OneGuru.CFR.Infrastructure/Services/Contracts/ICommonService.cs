using OneGuru.CFR.Domain.RequestModel;
using OneGuru.CFR.Persistence.EntityFrameworkDataAccess;
using System.Net.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace OneGuru.CFR.Infrastructure.Services.Contracts
{
    public interface ICommonService
    {
        string LoggedInUserEmail { get; }
        string UserToken { get; }
        string TenantId { get; }
        bool IsTokenActive { get; }
        IDistributedCache DistributedCache { get; }
        HttpClient GetHttpClient(string baseUrl);
        UserIdentity GetUserIdentity();
        Task ImpersonateAuditLog(AuditLogRequest auditLogRequest);

    }
}
