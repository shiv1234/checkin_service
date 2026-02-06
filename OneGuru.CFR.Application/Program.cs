#nullable enable
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Polly;
using System.Diagnostics.CodeAnalysis;
using OneGuru.CFR.Application;
using OneGuru.CFR.Application.Authorization;
using OneGuru.CFR.Application.Extensions;
using OneGuru.CFR.Application.Filters;
using OneGuru.CFR.Domain.Common;
using OneGuru.CFR.Domain.Ports;
using OneGuru.CFR.Domain.ResponseModels;
using OneGuru.CFR.Infrastructure.Adapters.BaseAdapter;
using OneGuru.CFR.Infrastructure.AutoMapper;
using OneGuru.CFR.Infrastructure.Messaging;
using OneGuru.CFR.Infrastructure.Services;
using OneGuru.CFR.Infrastructure.Services.Contracts;
using OneGuru.CFR.Persistence.EntityFrameworkDataAccess;
using OneGuru.CFR.Persistence.EntityFrameworkDataAccess.Entities;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Azure Key Vault
        ConfigureKeyVault(builder);

        // Configure Services
        ConfigureServices(builder);

        var app = builder.Build();

        // Configure Middleware Pipeline
        ConfigureMiddleware(app);

        app.Run();
    }

    private static void ConfigureKeyVault(WebApplicationBuilder builder)
    {
        var kvUrl = builder.Configuration["KeyVaultConfig:KVUrl"];
        var kvTenantId = builder.Configuration["KeyVaultConfig:TenantId"];
        var kvClientId = builder.Configuration["KeyVaultConfig:ClientId"];
        var kvClientSecretId = builder.Configuration["KeyVaultConfig:ClientSecretId"];

        if (string.IsNullOrEmpty(kvUrl) ||
            string.IsNullOrEmpty(kvTenantId) ||
            string.IsNullOrEmpty(kvClientId) ||
            string.IsNullOrEmpty(kvClientSecretId) ||
            !Guid.TryParse(kvTenantId, out _))
        {
            return;
        }

        var credentials = new ClientSecretCredential(kvTenantId, kvClientId, kvClientSecretId);
        var client = new SecretClient(new Uri(kvUrl), credentials);
        builder.Configuration.AddAzureKeyVault(client, new AzureKeyVaultConfigurationOptions
        {
            ReloadInterval = TimeSpan.FromMinutes(15)
        });
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        // HTTP Context
        services.AddHttpContextAccessor();

        // Logging
        services.AddLogging();

        // Redis Cache
        var redisConnection = configuration.GetValue<string>("Redis:ConnectionString");
        if (!string.IsNullOrEmpty(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = configuration.GetValue<string>("Redis:InstanceName");
            });
        }

        // Entity Framework - Multi-tenant DbContext
        ConfigureDatabase(services, configuration);

        // Azure AD Authentication
        ConfigureAuthentication(services, configuration);

        // Authorization Policies
        ConfigureAuthorization(services);

        // API Versioning
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Api-Version"));
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            cfg.RegisterServicesFromAssembly(AppDomain.CurrentDomain.Load("OneGuru.CFR.Infrastructure"));
        });

        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowOrigin", policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
        });

        // Controllers with Filters
        services.AddControllers(options =>
        {
            options.Filters.Add<ExceptionFilterAttribute>();
            options.Filters.Add<ResultFilter>();
        });

        // Swagger
        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<CustomHeaderSwaggerFilterAttribute>();
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "CFR Service APIs",
                Description = "Check-in Feedback Review (CFR) Microservice APIs"
            });

            // Add /cfr server prefix only for deployed environments (behind API gateway)
            if (!builder.Environment.IsDevelopment())
            {
                c.AddServer(new OpenApiServer { Url = "/cfr" });
            }

            // Add JWT Authentication to Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // Application Services
        RegisterApplicationServices(services);

        // Infrastructure Services - Messaging
        RegisterMessagingServices(services, configuration);

        // Application Insights
        var appInsightsKey = configuration.GetValue<string>("ApplicationInsights:InstrumentationKey");
        if (!string.IsNullOrEmpty(appInsightsKey))
        {
#pragma warning disable CS0618 // InstrumentationKey is deprecated but kept for backward compat
            services.AddApplicationInsightsTelemetry(appInsightsKey);
#pragma warning restore CS0618
        }

        // Health Checks
        services.AddHealthChecks()
            .AddCheck<ApplicationHealthCheck>("Application Health Check");
    }

    private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var keyVault = new DatabaseVaultResponse();

        services.AddDbContext<CfrContext>((serviceProvider, options) =>
        {
            var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
            if (httpContextAccessor?.HttpContext != null)
            {
                var httpContext = httpContextAccessor.HttpContext;
                var hasTenant = httpContext.Request.Headers.TryGetValue("TenantId", out var tenantId);

                if (!hasTenant && httpContext.Request.Host.Value?.Contains("localhost") == true)
                {
                    tenantId = configuration.GetValue<string>("TenantId");
                }

                if (!string.IsNullOrEmpty(tenantId))
                {
                    var tenantString = Encryption.DecryptRijndael(tenantId!, AppConstants.EncryptionPrivateKey);
                    var key = tenantString + "-Connection" + configuration.GetValue<string>("KeyVaultConfig:PostFix");
                    keyVault.ConnectionString = configuration.GetValue<string>(key);

                    if (keyVault.ConnectionString != null)
                    {
                        var retryPolicy = Policy.Handle<Exception>().Retry(2, (ex, count, context) =>
                        {
                            (configuration as IConfigurationRoot)?.Reload();
                            keyVault.ConnectionString = configuration.GetValue<string>(key);
                        });

                        retryPolicy.Execute(() =>
                        {
                            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                                   .UseSqlServer(keyVault.ConnectionString);
                        });
                    }
                }
            }
        });
    }

    private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        // Azure AD Authentication with Microsoft Identity Web
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(options =>
            {
                configuration.Bind("AzureAd", options);
                options.TokenValidationParameters.ValidateIssuer = true;
                options.TokenValidationParameters.ValidateAudience = true;
                options.TokenValidationParameters.ValidateLifetime = true;
                options.TokenValidationParameters.ValidateIssuerSigningKey = true;

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        // Additional token validation logic can be added here
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogError(context.Exception, "Authentication failed");
                        return Task.CompletedTask;
                    }
                };
            }, options =>
            {
                configuration.Bind("AzureAd", options);
            });
    }

    private static void ConfigureAuthorization(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // CFR Domain-specific policies
            options.AddPolicy(AuthorizationPolicies.CanViewCheckins, policy =>
                policy.RequireAuthenticatedUser()
                      .RequireClaim("permissions", "checkin.view"));

            options.AddPolicy(AuthorizationPolicies.CanCreateCheckins, policy =>
                policy.RequireAuthenticatedUser()
                      .RequireClaim("permissions", "checkin.create"));

            options.AddPolicy(AuthorizationPolicies.CanEditCheckins, policy =>
                policy.RequireAuthenticatedUser()
                      .RequireClaim("permissions", "checkin.edit"));

            options.AddPolicy(AuthorizationPolicies.CanViewTeamDashboard, policy =>
                policy.RequireAuthenticatedUser()
                      .RequireClaim("permissions", "team.dashboard.view"));

            options.AddPolicy(AuthorizationPolicies.CanManageTeam, policy =>
                policy.RequireAuthenticatedUser()
                      .RequireClaim("permissions", "team.manage"));

            // Role-based policies
            options.AddPolicy(AuthorizationPolicies.RequireManagerRole, policy =>
                policy.RequireAuthenticatedUser()
                      .RequireRole("Manager", "Admin"));

            options.AddPolicy(AuthorizationPolicies.RequireAdminRole, policy =>
                policy.RequireAuthenticatedUser()
                      .RequireRole("Admin"));
        });
    }

    private static void RegisterApplicationServices(IServiceCollection services)
    {
        services.AddScoped<IOperationStatus, OperationStatus>();
        services.AddScoped<ICfrDbContext>(opt => opt.GetRequiredService<CfrDbContext>());
        services.AddScoped<IUnitOfWorkAsync, UnitOfWork>();
        services.AddTransient<IServicesAggregator, ServicesAggregator>();
        services.AddTransient<OneGuru.CFR.Domain.Ports.ILogger, Logger>();
        services.AddTransient<ICommonBase, CommonBase>();
        services.AddTransient<TokenManagerMiddleware>();
        services.AddTransient<IKeyVaultService, KeyVaultService>();
        services.AddTransient<ISystemService, SystemService>();
        services.AddTransient<ICommonService, CommonService>();
    }

    private static bool IsValidConnectionString(string? connectionString)
    {
        return !string.IsNullOrEmpty(connectionString) &&
               !connectionString.Contains("YOUR_", StringComparison.OrdinalIgnoreCase);
    }

    private static void RegisterMessagingServices(IServiceCollection services, IConfiguration configuration)
    {
        // Azure Service Bus
        var serviceBusConnection = configuration.GetValue<string>("AzureServiceBus:ConnectionString");
        if (IsValidConnectionString(serviceBusConnection))
        {
            services.AddSingleton<IEventPublisher>(sp =>
                new ServiceBusEventPublisher(
                    serviceBusConnection!,
                    sp.GetRequiredService<ILogger<ServiceBusEventPublisher>>()));
        }
        else
        {
            services.AddSingleton<IEventPublisher, NullEventPublisher>();
        }

        // Azure Queue Storage
        var queueStorageConnection = configuration.GetValue<string>("AzureQueueStorage:ConnectionString");
        if (IsValidConnectionString(queueStorageConnection))
        {
            services.AddSingleton<IBackgroundJobQueue>(sp =>
                new AzureQueueBackgroundProcessor(
                    queueStorageConnection!,
                    sp.GetRequiredService<ILogger<AzureQueueBackgroundProcessor>>()));

            services.AddHostedService<QueueProcessorHostedService>();
        }
        else
        {
            services.AddSingleton<IBackgroundJobQueue, InMemoryBackgroundJobQueue>();
        }

        // Audit Service
        services.AddScoped<IAuditService, AuditService>();
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/error");
            app.UseHsts();
        }

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            // Use relative path so it works both locally and behind API gateway
            c.SwaggerEndpoint("v1/swagger.json", "CFR Service APIs");
        });

        app.UseCors("AllowOrigin");
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Custom Token Middleware (for legacy support)
        app.UseMiddleware<TokenManagerMiddleware>();

        // Health Check Endpoint
        app.MapHealthChecks("/health");

        // API Endpoints
        app.MapControllers();
    }
}
