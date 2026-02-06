using OneGuru.CFR.Domain.Common;
using System.Collections.Generic;

namespace OneGuru.CFR.Domain.RequestModel
{
    public class AzureBusPayload<T> where T : class
    {
        public ClientDetail ClientDetail { get; set; }
        public AzureBusServiceName AzureBusServiceName { get; set; }
        public T Data { get; set; }
        public List<T> DataList { get; set; }
        public string QueueName { get; set; }
    }

    public class ClientDetail
    {
        public string Token { get; set; }
        public string TenantId { get; set; }
        public string OriginHost { get; set; }
        public string BaseUrl { get; set; }
        public string UserIdentity { get; set; }

    }
}
