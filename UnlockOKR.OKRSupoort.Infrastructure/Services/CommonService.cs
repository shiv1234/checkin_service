using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using UnlockOKR.OKRSupoort.Domain.Common;
using UnlockOKR.OKRSupoort.Domain.RequestModel;
using UnlockOKR.OKRSupoort.Domain.ResponseModels;
using UnlockOKR.OKRSupoort.Infrastructure.Services.Contracts;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace UnlockOKR.OKRSupoort.Infrastructure.Services
{
    public class CommonService : BaseService, ICommonService
    {
        private readonly IServicesAggregator _servicesAggregateService;
        public ISystemService _systemService { get; set; }
        private ServiceBusClient _client;
        private ServiceBusSender _clientSender;

        [Obsolete]
        public CommonService(IServicesAggregator servicesAggregateService, ISystemService systemService) : base(servicesAggregateService)
        {
            _servicesAggregateService = servicesAggregateService;
            _systemService = systemService;
        }

        public UserIdentity GetUserIdentity()
        {
            var loginUserDetail = new UserIdentity();
            var hasIdentity = _systemService.HttpContext.Request.Headers.TryGetValue("UserIdentity", out var userIdentity);

            if (!hasIdentity)
            {
                using var httpClient = GetHttpClient(Configuration.GetSection("OkrUser:BaseUrl").Value);
                using var response = httpClient.GetAsync($"Identity");
                if (!response.Result.IsSuccessStatusCode)
                    return loginUserDetail;
                var apiResponse = response.Result.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<Payload<UserIdentity>>(apiResponse.Result);
                if (user != null)
                    loginUserDetail = user.Entity;
            }
            else
            {
                var decryptVal = Encryption.DecryptStringAes(userIdentity, AppConstants.EncryptionSecretKey, AppConstants.EncryptionSecretIvKey);
                if (decryptVal != null)
                    loginUserDetail = JsonConvert.DeserializeObject<UserIdentity>(decryptVal);
            }

            return loginUserDetail;
        }

        public HttpClient GetHttpClient(string baseUrl)
        {
            string identity = string.Empty;
            var hasIdentity = _systemService.HttpContext.Request.Headers.TryGetValue("UserIdentity", out var userIdentity);
            if (hasIdentity)
            {
                identity = userIdentity;
            }
            var hasTenant = _systemService.HttpContext.Request.Headers.TryGetValue("TenantId", out var tenantId);
            if ((!hasTenant && _systemService.HttpContext.Request.Host.Value.Contains("localhost")))
                tenantId = _servicesAggregateService.Configuration.GetValue<string>("TenantId");
            string domain;
            var hasOrigin = _systemService.HttpContext.Request.Headers.TryGetValue("OriginHost", out var origin);
            if (!hasOrigin && _systemService.HttpContext.Request.Host.Value.Contains("localhost"))
                domain = _servicesAggregateService.Configuration.GetValue<string>("FrontEndUrl");
            else
                domain = string.IsNullOrEmpty(origin) ? string.Empty : origin.ToString();
            HttpClient httpClient = _systemService.SystemHttpClient();

            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + UserToken);
            httpClient.DefaultRequestHeaders.Add("TenantId", tenantId.ToString());
            httpClient.DefaultRequestHeaders.Add("OriginHost", domain);
            httpClient.DefaultRequestHeaders.Add("UserIdentity", identity);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }

        #region BaseService
        private IDistributedCache _distributedCache;
        public IDistributedCache DistributedCache => _distributedCache ??= _systemService.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
        public string LoggedInUserEmail => _systemService.HttpContext.User.Identities.FirstOrDefault()?.Claims.FirstOrDefault(x => x.Type == "email")?.Value;

        public string UserToken => _systemService.HttpContext.User.Identities.FirstOrDefault()?.Claims.FirstOrDefault(x => x.Type == "token")?.Value;

        public string TenantId => _systemService.HttpContext.User.Identities.FirstOrDefault()?.Claims.FirstOrDefault(x => x.Type == "tenantId")?.Value;

        public bool IsTokenActive => (!string.IsNullOrEmpty(LoggedInUserEmail) && !string.IsNullOrEmpty(UserToken));

        #endregion

        public ClientDetail SetClientDetail()
        {
            string identity = string.Empty;
            var hasIdentity = _systemService.HttpContext.Request.Headers.TryGetValue("UserIdentity", out var userIdentity);
            if (hasIdentity)
            {
                identity = userIdentity;
            }
            string domain;
            var hasOrigin = _systemService.HttpContext.Request.Headers.TryGetValue("OriginHost", out var origin);
            if (!hasOrigin && _systemService.HttpContext.Request.Host.Value.Contains("localhost"))
                domain = Configuration.GetValue<string>("FrontEndUrl").ToString();
            else
                domain = string.IsNullOrEmpty(origin) ? string.Empty : origin.ToString();

            var hasTenant = _systemService.HttpContext.Request.Headers.TryGetValue("TenantId", out var tenantId);
            if ((!hasTenant && _systemService.HttpContext.Request.Host.Value.Contains("localhost")))
                tenantId = Configuration.GetValue<string>("TenantId");

            var clientDetail = new ClientDetail()
            {
                OriginHost = domain,
                Token = UserToken,
                TenantId = tenantId,
                UserIdentity = identity
            };
            return clientDetail;
        }

        public async Task ImpersonateAuditLog(AuditLogRequest auditLogRequest)
        {
            var clientDetail = SetClientDetail();
            clientDetail.BaseUrl = _servicesAggregateService.Configuration.GetValue<string>("ReportService:BaseUrl");
            var payload = new AzureBusPayload<AuditLogRequest>
            {
                Data = auditLogRequest,
                AzureBusServiceName = AzureBusServiceName.AuditLog,
                ClientDetail = clientDetail,
                QueueName = AppConstants.QueueAuditLog
            };
            _client = new ServiceBusClient(_servicesAggregateService.Configuration.GetValue<string>("AzureServiceBus:ConnectionString"));
            _clientSender = _client.CreateSender(AppConstants.EmailTopicName);
            var message = new ServiceBusMessage(System.Text.Json.JsonSerializer.Serialize(payload));
            await _clientSender.SendMessageAsync(message).ConfigureAwait(false);
        }
    }
}
