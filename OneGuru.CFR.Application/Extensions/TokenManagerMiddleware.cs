using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OneGuru.CFR.Domain.Common;
using OneGuru.CFR.Domain.RequestModel;
using OneGuru.CFR.Infrastructure.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OneGuru.CFR.Application.Extensions
{
    public class TokenManagerMiddleware : IMiddleware
    {
        private readonly IConfiguration _configuration;
        private readonly ISystemService _systemService;

        public TokenManagerMiddleware(IConfiguration configuration, ISystemService systemService)
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
                        // No token at all — pass through, let [Authorize]/[AllowAnonymous] decide
                        await next(context);
                        return;
                    }
                }

                var token = string.Empty;
                if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authorization.Substring("Bearer ".Length).Trim();
                }

                if (string.IsNullOrEmpty(token))
                {
                    await next(context);
                    return;
                }

                try
                {
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
                        string LoggedInUserEmail = context?.User.Identities.FirstOrDefault()?.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
                        if (expiryDateTimeUtc < DateTime.UtcNow)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            var myByteArray = Encoding.UTF8.GetBytes("TokenExpired");
                            await context.Response.Body.WriteAsync(myByteArray, 0, myByteArray.Length);
                            return;
                        }
                    }
                }
                catch (Exception)
                {
                    // If legacy token processing fails, pass through — let [Authorize]/[AllowAnonymous] decide
                    await next(context);
                    return;
                }
            }
            await next(context);
        }

        private ClaimsIdentity GetAllClaims(JwtSecurityToken principal, HttpContext context, string token)
        {
            UserIdentity userIdentity = GetUserIdentity(context);
            string email = string.Empty;
            string name = string.Empty;
            bool isImpersonatedUser = false;
            string impersonatedBy = string.Empty;
            string impersonatedByUserName = string.Empty;
            long impersonatedById = 0;
            long impersonatedUserId = 0;
            bool isSSO = true;
            string encryptIdentity = string.Empty;

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

            if (IsIssuerUnlockTalent(principal))
            {
                isSSO = false;
            }

            if (string.IsNullOrEmpty(email))
            {
                if (userIdentity == null || string.IsNullOrEmpty(userIdentity?.EmailId))
                {
                    email = "";
                }
                else
                {
                    email = userIdentity.EmailId;
                    name = userIdentity.FirstName + " " + userIdentity.LastName;
                }
            }

            if (userIdentity.IsImpersonatedUser)
            {
                email = userIdentity.EmailId;
                name = userIdentity.FirstName + " " + userIdentity.LastName;
                isImpersonatedUser = true;
                impersonatedBy = userIdentity.ImpersonatedBy;
                impersonatedById = userIdentity.ImpersonatedById;
                impersonatedUserId = userIdentity.EmployeeId;
            }

            var claimList = new List<Claim>
                {
                    new Claim("email", email),
                    new Claim("name", name),
                    new Claim("token", token),
                    new Claim("tenantId", GetTenantCliams(context)),
                    new Claim("origin", GetOriginCliams(context)),
                    new Claim("isImpersonateUser", isImpersonatedUser.ToString(), ClaimValueTypes.Boolean),
                    new Claim("impersonateBy", impersonatedBy),
                    new Claim("impersonateById", impersonatedById.ToString(), ClaimValueTypes.Integer64),
                    new Claim("impersonateByUserName", impersonatedByUserName),
                    new Claim("impersonateUserId", impersonatedUserId.ToString(), ClaimValueTypes.Integer64),
                    new Claim("isSSO", isSSO.ToString(), ClaimValueTypes.Boolean),
                    new Claim("encryptIdentity", GetUserIdentityClaims(context)),
                    new Claim("employeeId", userIdentity.EmployeeId.ToString(), ClaimValueTypes.Integer64)
                };

            return new ClaimsIdentity(claimList, "Bearer");
        }

        private UserIdentity GetUserIdentity(HttpContext httpContext)
        {
            try
            {
                var hasIdentity = httpContext.Request.Headers.TryGetValue("UserIdentity", out var userIdentity);
                UserIdentity identity = new UserIdentity();
                if (hasIdentity)
                {
                    var decryptVal = Encryption.DecryptStringAes(userIdentity, AppConstants.EncryptionSecretKey, AppConstants.EncryptionSecretIvKey);
                    if (string.IsNullOrEmpty(decryptVal)) return identity;
                    identity = JsonConvert.DeserializeObject<UserIdentity>(decryptVal);
                }
                return identity;
            }
            catch (Exception)
            {
                return new UserIdentity();
            }
        }

        private string GetUserIdentityClaims(HttpContext context)
        {
            string encryptUserIdentity = string.Empty;
            var hasIdentity = context.Request.Headers.TryGetValue("UserIdentity", out var userIdentity);
            if (hasIdentity && !string.IsNullOrEmpty(userIdentity) && userIdentity != "null")
            {
                encryptUserIdentity = userIdentity;
            }
            return encryptUserIdentity;
        }

        private string GetOriginCliams(HttpContext context)
        {
            var hasOrigin = context.Request.Headers.TryGetValue("OriginHost", out var origin);
            if (!hasOrigin && context.Request.Host.Value.Contains("localhost"))
                origin = new Uri(_configuration.GetValue<string>("FrontEndUrl")).Host;

            if (hasOrigin)
                origin = new Uri(origin).Host;

            return origin;
        }

        private string GetTenantCliams(HttpContext context)
        {
            var hasTenant = context.Request.Headers.TryGetValue("TenantId", out var tenantId);
            if (!hasTenant && context.Request.Host.Value.Contains("localhost") || string.IsNullOrEmpty(tenantId) || tenantId == "null")
                tenantId = _configuration.GetValue<string>("TenantId");
            else
                tenantId = Encryption.DecryptRijndael(tenantId, AppConstants.EncryptionPrivateKey);

            return tenantId;
        }

        private bool IsIssuerUnlockTalent(JwtSecurityToken principal)
        {
            bool isDBLogin = false;
            string issuerSub = principal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
            if (!string.IsNullOrEmpty(issuerSub) && issuerSub == "DBLogin")
            {
                isDBLogin = true;
            }
            return isDBLogin;
        }
    }
}
