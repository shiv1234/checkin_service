using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using UnlockOKR.OKRSupoort.Domain.Common;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UnlockOKR.OKRSupoort.Infrastructure.Services.Contracts;
using UnlockOKR.OKRSupoort.Domain.RequestModel;
using Newtonsoft.Json;

namespace UnlockOKR.OKRSupoort.Application.Extensions
{
    public class TokenManagerMiddleware : IMiddleware
    {

        private readonly IConfiguration _configuration;
        private readonly ISystemService _systemService;

        public TokenManagerMiddleware(IConfiguration configuration , ISystemService systemService)
        {
            _configuration = configuration;
            _systemService = systemService;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.Request.Path.Value.Contains("health"))
            {
                string authorization = context.Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authorization))
                {
                    authorization = context.Request.Headers["Token"];
                    if (string.IsNullOrEmpty(authorization))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return;
                    }
                }

                var token = string.Empty;
                if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authorization.Substring("Bearer ".Length).Trim();

                }
                // If no token found, no further work possible
                if (string.IsNullOrEmpty(token))
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                var stream = token;
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(stream);

                var principal = jsonToken as JwtSecurityToken;
                if (principal != null)
                {
                    var expiryDateUnix = long.Parse(principal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                    var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix);
                    var claimIdentity = GetAllClaims(principal, context, token);
                    context.User = new ClaimsPrincipal(claimIdentity);
                    if (expiryDateTimeUtc < DateTime.UtcNow)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        var myByteArray = Encoding.UTF8.GetBytes("TokenExpired");
                        await context.Response.Body.WriteAsync(myByteArray, 0, myByteArray.Length);
                        return;
                    }
                }
            }
            await next(context);
        }

        private ClaimsIdentity GetAllClaims(JwtSecurityToken principal, HttpContext context, string token)
        {
            UserIdentity userIdentity = GetUserIdentity(context);
            string email = string.Empty;
            string name = string.Empty;
            bool isImpersonateUser = false;
            string impersonateBy = string.Empty;
            long impersonateById = 0;

            if (userIdentity.IsImpersonatedUser)
            {
                email = userIdentity.EmailId;
                name = userIdentity.FirstName + " " + userIdentity.LastName;
                isImpersonateUser = true;
                impersonateBy = userIdentity.ImpersonatedBy;
                impersonateById = userIdentity.ImpersonatedById;
            }
            else
            {
                var emailClaim = principal.Claims.FirstOrDefault(x => x.Type == "email");
                if (emailClaim != null && !string.IsNullOrEmpty(emailClaim.Value))
                    email = emailClaim.Value;
                else
                {
                    var preferredClaim = principal.Claims.FirstOrDefault(x => x.Type == "preferred_username");
                    if (preferredClaim != null && !string.IsNullOrEmpty(preferredClaim.Value))
                        email = preferredClaim.Value;
                    else
                    {
                        var uniqueClaim = principal.Claims.FirstOrDefault(x => x.Type == "unique_name");
                        if (uniqueClaim != null && !string.IsNullOrEmpty(uniqueClaim.Value))
                            email = uniqueClaim.Value;
                    }
                }

                if (principal.Claims.FirstOrDefault(x => x.Type == "name") != null)
                    name = principal.Claims.FirstOrDefault(x => x.Type == "name")?.Value;
            }

            var hasTenant = context.Request.Headers.TryGetValue("TenantId", out var tenantId);
            if (!hasTenant && context.Request.Host.Value.Contains("localhost"))
                tenantId = _configuration.GetValue<string>("TenantId");

            if (hasTenant)
                tenantId = Encryption.DecryptRijndael(tenantId, AppConstants.EncryptionPrivateKey);

            var hasOrigin = context.Request.Headers.TryGetValue("OriginHost", out var origin);
            if (!hasOrigin && context.Request.Host.Value.Contains("localhost"))
                origin = _systemService.SystemUri(_configuration.GetValue<string>("FrontEndUrl")).Host;

            if (hasOrigin)
                origin = _systemService.SystemUri(origin).Host;

            var claimList = new List<Claim>
                {
                    new Claim("email", email),
                    new Claim("name", name),
                    new Claim("token", token),
                    new Claim("tenantId", tenantId),
                    new Claim("origin", origin),
                    new Claim("isImpersonateUser",isImpersonateUser.ToString(),ClaimValueTypes.Boolean),
                    new Claim("impersonateBy", impersonateBy),
                    new Claim("impersonateById", impersonateById.ToString(),ClaimValueTypes.Integer64)
                };

            return new ClaimsIdentity(claimList);
        }

        private UserIdentity GetUserIdentity(HttpContext httpContext)
        {
            var hasIdentity = httpContext.Request.Headers.TryGetValue("UserIdentity", out var userIdentity);
            UserIdentity identity = new UserIdentity();
            if (hasIdentity)
            {
                var decryptVal = Encryption.DecryptStringAes(userIdentity, AppConstants.EncryptionSecretKey, AppConstants.EncryptionSecretIvKey);
                if (string.IsNullOrEmpty(decryptVal)) return identity;
                identity = JsonConvert.DeserializeObject<UserIdentity>(decryptVal);
            }
            else
            {
                //TO DO
            }
            return identity;
        }
    }
}
