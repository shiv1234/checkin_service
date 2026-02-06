using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OneGuru.CFR.Domain.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using OneGuru.CFR.Infrastructure.Services.Contracts;
using OneGuru.CFR.Domain.RequestModel;

namespace OneGuru.CFR.Application.Extensions;

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
        // Skip authentication for health checks and swagger
        if (context.Request.Path.Value?.Contains("health") == true ||
            context.Request.Path.Value?.Contains("swagger") == true)
        {
            await next(context);
            return;
        }

        // If already authenticated via Azure AD, proceed
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            await next(context);
            return;
        }

        // Legacy token validation for backwards compatibility
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
            token = authorization["Bearer ".Length..].Trim();
        }

        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);

            if (jsonToken is JwtSecurityToken principal)
            {
                var expiryDateUnix = long.Parse(principal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expiryDateTimeUtc = DateTimeOffset.FromUnixTimeSeconds(expiryDateUnix).UtcDateTime;

                if (expiryDateTimeUtc < DateTime.UtcNow)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    var myByteArray = Encoding.UTF8.GetBytes("TokenExpired");
                    await context.Response.Body.WriteAsync(myByteArray);
                    return;
                }

                var claimIdentity = GetAllClaims(principal, context, token);
                context.User = new ClaimsPrincipal(claimIdentity);
            }
        }
        catch (Exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
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
            email = userIdentity.EmailId ?? string.Empty;
            name = $"{userIdentity.FirstName} {userIdentity.LastName}";
            isImpersonateUser = true;
            impersonateBy = userIdentity.ImpersonatedBy ?? string.Empty;
            impersonateById = userIdentity.ImpersonatedById;
        }
        else
        {
            var emailClaim = principal.Claims.FirstOrDefault(x => x.Type == "email");
            if (emailClaim != null && !string.IsNullOrEmpty(emailClaim.Value))
            {
                email = emailClaim.Value;
            }
            else
            {
                var preferredClaim = principal.Claims.FirstOrDefault(x => x.Type == "preferred_username");
                if (preferredClaim != null && !string.IsNullOrEmpty(preferredClaim.Value))
                {
                    email = preferredClaim.Value;
                }
                else
                {
                    var uniqueClaim = principal.Claims.FirstOrDefault(x => x.Type == "unique_name");
                    if (uniqueClaim != null && !string.IsNullOrEmpty(uniqueClaim.Value))
                    {
                        email = uniqueClaim.Value;
                    }
                }
            }

            name = principal.Claims.FirstOrDefault(x => x.Type == "name")?.Value ?? string.Empty;
        }

        var hasTenant = context.Request.Headers.TryGetValue("TenantId", out var tenantId);
        if (!hasTenant && context.Request.Host.Value.Contains("localhost"))
        {
            tenantId = _configuration.GetValue<string>("TenantId");
        }

        if (hasTenant && !string.IsNullOrEmpty(tenantId))
        {
            tenantId = Encryption.DecryptRijndael(tenantId!, AppConstants.EncryptionPrivateKey);
        }

        var hasOrigin = context.Request.Headers.TryGetValue("OriginHost", out var origin);
        if (!hasOrigin && context.Request.Host.Value.Contains("localhost"))
        {
            var frontEndUrl = _configuration.GetValue<string>("FrontEndUrl");
            if (!string.IsNullOrEmpty(frontEndUrl))
            {
                origin = _systemService.SystemUri(frontEndUrl).Host;
            }
        }

        if (hasOrigin && !string.IsNullOrEmpty(origin))
        {
            origin = _systemService.SystemUri(origin!).Host;
        }

        var claimList = new List<Claim>
        {
            new("email", email),
            new("name", name),
            new("token", token),
            new("tenantId", tenantId.ToString() ?? string.Empty),
            new("origin", origin.ToString() ?? string.Empty),
            new("isImpersonateUser", isImpersonateUser.ToString(), ClaimValueTypes.Boolean),
            new("impersonateBy", impersonateBy),
            new("impersonateById", impersonateById.ToString(), ClaimValueTypes.Integer64)
        };

        return new ClaimsIdentity(claimList, "Bearer");
    }

    private UserIdentity GetUserIdentity(HttpContext httpContext)
    {
        var hasIdentity = httpContext.Request.Headers.TryGetValue("UserIdentity", out var userIdentity);
        var identity = new UserIdentity();

        if (hasIdentity && !string.IsNullOrEmpty(userIdentity))
        {
            var decryptVal = Encryption.DecryptStringAes(userIdentity!, AppConstants.EncryptionSecretKey, AppConstants.EncryptionSecretIvKey);
            if (!string.IsNullOrEmpty(decryptVal))
            {
                identity = JsonSerializer.Deserialize<UserIdentity>(decryptVal) ?? new UserIdentity();
            }
        }

        return identity;
    }
}
