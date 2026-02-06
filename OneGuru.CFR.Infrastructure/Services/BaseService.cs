using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OneGuru.CFR.Domain.Common;
using OneGuru.CFR.Domain.Ports;
using OneGuru.CFR.Domain.RequestModel;
using OneGuru.CFR.Domain.ResponseModels;
using OneGuru.CFR.Infrastructure.Services.Contracts;
using OneGuru.CFR.Persistence.EntityFrameworkDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace OneGuru.CFR.Infrastructure.Services
{
    public class BaseService : IBaseService
    {
        private readonly ILogger<BaseService> _logger;

        [Obsolete("IHostingEnvironment is Obsolete now")]
        public BaseService(IServicesAggregator servicesAggregateService)
        {
            UnitOfWorkAsync = servicesAggregateService.UnitOfWorkAsync;
            OperationStatus = servicesAggregateService.OperationStatus;
            Mapper = servicesAggregateService.Mapper;
            LoggerFactory = servicesAggregateService.LoggerFactory;
            Configuration = servicesAggregateService.Configuration;
            HostingEnvironment = (IHostingEnvironment)servicesAggregateService.HostingEnvironment;
            _logger = LoggerFactory.CreateLogger<BaseService>();
        }
        public IUnitOfWorkAsync UnitOfWorkAsync { get; set; }
        public IOperationStatus OperationStatus { get; set; }
        protected IMapper Mapper { get; private set; }
        public ILoggerFactory LoggerFactory { get; set; }

        public Payload<T> GetPayloadStatus<T>(Payload<T> payload, string key = null, string errorMessage = null)
        {
            if (payload != null)
            {
                if (!string.IsNullOrEmpty(key) && !payload.MessageList.ContainsKey(key))
                {
                    payload.MessageList.Add(key, errorMessage);
                }
                if (payload.MessageList.Count == 0)
                {
                    payload.IsSuccess = true;
                    payload.Status = (int)HttpStatusCode.OK;
                    payload.MessageType = MessageType.Success.ToString();
                }
                else
                {
                    payload.IsSuccess = false;
                    payload.Status = (int)HttpStatusCode.BadRequest;
                    payload.MessageType = MessageType.Error.ToString();
                }
            }

            return payload;
        }
        public Payload<T> GetPayloadStatusSuccess<T>(Payload<T> payload, string key = null, string errorMessage = null)
        {
            if (payload != null)
            {
                if (!string.IsNullOrEmpty(key) && !payload.MessageList.ContainsKey(key))
                {
                    payload.MessageList.Add(key, errorMessage);
                }
                payload.IsSuccess = true;
                payload.Status = (int)HttpStatusCode.OK;
                payload.MessageType = MessageType.Success.ToString();
            }
            return payload;
        }
        public Payload<T> GetPayloadStatusNoContent<T>(Payload<T> payload, string key = null, string errorMessage = null)
        {
            if (payload != null)
            {
                if (!string.IsNullOrEmpty(key) && !payload.MessageList.ContainsKey(key))
                {
                    payload.MessageList.Add(key, errorMessage);
                }
                payload.IsSuccess = true;
                payload.Status = (int)HttpStatusCode.NoContent;
                payload.MessageType = MessageType.Success.ToString();
            }
            return payload;
        }

        public class PayloadCustomList<T>
        {
            public T Entity { get; set; }


            /// <summary>
            /// Gets or sets the view model list.
            /// </summary>
            /// <value>
            /// The view model list.
            /// </value>
            public List<T> EntityList { get; set; }

            /// <summary>
            /// Gets or sets the message list.
            /// </summary>
            /// <value>
            /// The message list.
            /// </value>
            public Dictionary<string, string> MessageList { get; set; } = new Dictionary<string, string>();

            /// <summary>
            /// Gets or sets the type of the message.
            /// </summary>
            /// <value>
            /// The type of the message.
            /// </value>
            public string MessageType { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is success.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is success; otherwise, <c>false</c>.
            /// </value>
            public bool IsSuccess { get; set; } = false;

            public int Status { get; set; }

        }

        public class PageResults<T>
        {
            public int PageIndex { get; set; }
            public int PageSize { get; set; }
            public int TotalRecords { get; set; }
            public int TotalPages { get; set; }
            public int HeaderCode { get; set; }
            public List<T> Records { get; set; }
            public IEnumerable<T> Results { get; set; }
        }
        public CfrDbContext CfrDbContext { get; set; }
        public IConfiguration Configuration { get; set; }
        [Obsolete]
        public IHostingEnvironment HostingEnvironment { get; set; }
       
       
    }
}
