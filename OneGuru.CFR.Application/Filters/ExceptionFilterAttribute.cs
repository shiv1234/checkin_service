using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OneGuru.CFR.Domain.Common;
using OneGuru.CFR.Domain.RequestModel;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace OneGuru.CFR.Application.Filters
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExceptionFilterAttribute : Microsoft.AspNetCore.Mvc.Filters.ExceptionFilterAttribute
    {
        private readonly ILogger<ExceptionFilterAttribute> _logger;
        public IConfiguration Configuration { get; }
        public ExceptionFilterAttribute(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<ExceptionFilterAttribute>();
            Configuration = configuration;
        }
        public override void OnException(ExceptionContext context)
        {
            var controller = string.Empty;
            var action = string.Empty;
            var statusCode = HttpStatusCode.InternalServerError;
            var envName = Configuration.GetValue<string>("ApplicationInsights:EnviromentName");
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = (int)statusCode;
            var tenantId = Configuration.GetValue<string>("TenantId");
            var hasOrigin = context.HttpContext.Request.Headers.TryGetValue("OriginHost", out var origin);
            if (!hasOrigin && context.HttpContext.Request.Host.Value.Contains("localhost"))
                origin = new Uri(Configuration.GetValue<string>("FrontEndUrl")).Host;
            if (hasOrigin)
                origin = new Uri(origin).Host;
            var hasIdentity = context.HttpContext.Request.Headers.TryGetValue("UserIdentity", out var userIdentity);
            UserIdentity Identity = new UserIdentity();
            if (hasIdentity)
            {
                var decryptVal = Encryption.DecryptStringAes(userIdentity, AppConstants.EncryptionSecretKey, AppConstants.EncryptionSecretIvKey);
                Identity = JsonConvert.DeserializeObject<UserIdentity>(decryptVal);
            }
            if (context.RouteData != null)
            {
                action = context.RouteData.Values["action"].ToString();
                controller = context.RouteData.Values["controller"].ToString();
            }
            context.Result = new JsonResult(new
            {
                error = new[] { context.Exception.Message },
                stackTrace = context.Exception.StackTrace
            });
            context.ExceptionHandled = true;
            _logger.LogError("Service Name : Task, Env Name: " + envName + ", Login EmployeeId: " + Identity?.EmployeeId + ", TenantId: " + tenantId + ": " + "FrontendURL: " + origin + ": " + controller + "/" + action + ": " + context.Exception.ToString() + "InnerException: " + context.Exception.InnerException);

        }

    }
}
