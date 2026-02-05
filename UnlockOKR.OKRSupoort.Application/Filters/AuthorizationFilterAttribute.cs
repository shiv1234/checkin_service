using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using UnlockOKR.OKRSupoort.Domain.Common;
using UnlockOKR.OKRSupoort.Domain.RequestModel;
using UnlockOKR.OKRSupoort.Domain.ResponseModels;
using UnlockOKR.OKRSupoort.Infrastructure.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace UnlockOKR.OKRSupoort.Application.Filters
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizationFilterAttribute : TypeFilterAttribute
    {
        public AuthorizationFilterAttribute(Permissions permissions) : base(typeof(AuthorizeAction))
        {
            Arguments = new object[] {
                permissions
        };
        }
        public class AuthorizeAction : IAuthorizationFilter
        {
            private readonly IConfiguration _configuration;
            private readonly Permissions _permissions;
            private readonly ISystemService _systemService;


            public AuthorizeAction(Permissions permissions, IServicesAggregator servicesAggregateService , ISystemService systemService)
            {
                _permissions = permissions;
                _configuration = servicesAggregateService.Configuration;
                _systemService = systemService;

            }
            public void OnAuthorization(AuthorizationFilterContext context)
            {

                UserIdentity currentUserDetails;
                var payload = new Payload<UserIdentity>();

                string authorization = string.IsNullOrEmpty(context.HttpContext.Request.Headers["Authorization"]) ? context.HttpContext.Request.Headers["Token"] : context.HttpContext.Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authorization))
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }

                var token = (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    ? authorization["Bearer ".Length..].Trim()
                    : authorization;

                var hasIdentity = context.HttpContext.Request.Headers.TryGetValue("UserIdentity", out var userIdentity);

                if (!hasIdentity)
                {
                    //for localhost only
                    currentUserDetails = GetUserIdentity(token, context).Result;
                    currentUserDetails.RolePermissions = GetUserPermissions(token, context).Result;
                }
                else
                {
                    var decryptVal = Encryption.DecryptStringAes(userIdentity, AppConstants.EncryptionSecretKey,
                        AppConstants.EncryptionSecretIvKey);
                    currentUserDetails = JsonConvert.DeserializeObject<UserIdentity>(decryptVal);
                }

                if (currentUserDetails != null)
                {
                    if (currentUserDetails.RolePermissions.Find(x => x.PermissionId == (int)_permissions) == null)
                    {
                        payload.IsSuccess = false;
                        payload.Status = (int)HttpStatusCode.Unauthorized;
                        payload.MessageType = MessageType.Success.ToString();
                        payload.MessageList.Add("message", "Unauthorized User");
                    }
                    else
                    {
                        return;
                    }
                }
                context.Result = new JsonResult("NotAuthorized")
                {
                    Value = payload,
                };
            }
            public async Task<UserIdentity> GetUserIdentity(string jwtToken, AuthorizationFilterContext context)
            {
                var loginUserDetail = new UserIdentity();

                if (string.IsNullOrEmpty(jwtToken)) return loginUserDetail;
                using var httpClient = GetHttpClient(jwtToken, _configuration.GetValue<string>("OkrUser:BaseUrl"), context);
                using var response = await httpClient.GetAsync($"Identity");

                if (!response.IsSuccessStatusCode) return loginUserDetail;
                var apiResponse = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<Payload<UserIdentity>>(apiResponse);
                return user?.Entity;
            }

            private async Task<List<EmployeePermissionResponse>> GetUserPermissions(string jwtToken, AuthorizationFilterContext context)
            {
                var loginUserPermissions = new List<EmployeePermissionResponse>();

                if (string.IsNullOrEmpty(jwtToken)) return loginUserPermissions;
                using var httpClient = GetHttpClient(jwtToken, _configuration.GetValue<string>("OkrAuth:BaseUrl"), context);
                using var response = await httpClient.GetAsync($"employeeRole/permissions");

                if (!response.IsSuccessStatusCode) return loginUserPermissions;
                var apiResponse = await response.Content.ReadAsStringAsync();
                var permission = JsonConvert.DeserializeObject<Payload<EmployeeRolePermissionResponse>>(apiResponse);
                return permission?.Entity.EmployeePermissions;
            }

            private HttpClient GetHttpClient(string jwtToken, string baseUrl, AuthorizationFilterContext context)
            {
                var hasTenant = context.HttpContext.Request.Headers.TryGetValue("TenantId", out var tenantId);

                //for localhost
                if (!hasTenant && context.HttpContext.Request.Host.Value.Contains("localhost"))
                    tenantId = _configuration.GetValue<string>("TenantId");

                string domain;
                var hasOrigin = context.HttpContext.Request.Headers.TryGetValue("OriginHost", out var origin);

                //for localhost
                if (!hasOrigin && context.HttpContext.Request.Host.Value.Contains("localhost"))
                    domain = _configuration.GetValue<string>("FrontEndUrl");
                else
                    domain = string.IsNullOrEmpty(origin) ? string.Empty : origin.ToString();

                var httpClient = _systemService.SystemHttpClient();
                httpClient.BaseAddress = _systemService.SystemUri(baseUrl);

                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtToken);
                httpClient.DefaultRequestHeaders.Add("TenantId", tenantId.ToString());
                httpClient.DefaultRequestHeaders.Add("OriginHost", domain);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return httpClient;
            }
        }
    }
}
