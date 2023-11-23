using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
using Refit;
using System.Text;
using System.Text.Json.Serialization;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VGStandard.Application.Common.Web.Cache;
using VGStandard.Common.Web.Integrations;
using VGStandard.Common.Web.Options;
using VGStandard.Common.Web.Responses;
using VGStandard.Common.Web.Services;
using VGStandard.Core.Settings;

namespace VGStandard.Common.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppSettingsIoC(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSettings>(configuration);
        var settings = configuration.Get<AppSettings>();
        return services.AddSingleton(settings);
    }

    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        return services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }

    public static IServiceCollection AddTrackability(this IServiceCollection services)
    {
        //return services.AddScoped<ITrackable, Trackable>();
        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services, IWebHostEnvironment environment, string application)
    {

            services.AddVersioning()
                    .AddSwaggerVersioning(environment.ToString(), application);
        
        return services;
    }

    private static IServiceCollection AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(setup =>
        {
            setup.DefaultApiVersion = new ApiVersion(1, 0);
            setup.AssumeDefaultVersionWhenUnspecified = true;
            setup.UseApiBehavior = false;
            setup.ReportApiVersions = true;
        });

        services.AddVersionedApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    public static IServiceCollection AddVault(this IServiceCollection services, AppSettings settings)
    {
        services.AddSingleton<IVaultClient>(sp =>
        {
            var authMethod = new TokenAuthMethodInfo(settings.Tokens.Vault);
            var vaultClientSettings = new VaultClientSettings(settings.ConnectionStrings.Vault, authMethod);
            var vault = new VaultClient(vaultClientSettings);
            return vault;
        });

        services.AddSingleton<IVaultService, VaultService>();
        return services;
    }

    public static T BindTo<T>(this IConfiguration configuration, string key) where T : class, new()
    {
        T bindingObject = new T();

        configuration.GetSection(key).Bind(bindingObject);

        return bindingObject;
    }

    public static T BindTo<T>(this IConfiguration configuration) where T : class, new() => configuration.BindTo<T>(typeof(T).Name);


    public static void AddRestIntegrations(this WebApplicationBuilder builder, AppSettings settings)
    {
        WaitAndRetryConfig wrc = builder.Configuration.BindTo<WaitAndRetryConfig>();

        AsyncRetryPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>() // Thrown by Polly's TimeoutPolicy if the inner call gets timeout.
            .WaitAndRetryAsync(wrc.Retry, _ => TimeSpan.FromMilliseconds(wrc.Wait));

        AsyncTimeoutPolicy<HttpResponseMessage> timeoutPolicy = Policy
            .TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(wrc.Timeout));


        var oneSignalOptions = new RefitSettings()
        {
            AuthorizationHeaderValueGetter = (request, cancellationToken) =>
                Task.FromResult($"token={settings.Integrations.Single(i => i.Name == nameof(IOneSignal).Substring(1)).Token}")
        };

        var signalROptions = new RefitSettings()
        {
            AuthorizationHeaderValueGetter = (request, cancellationToken) =>
                Task.FromResult($"Bearer {settings.Integrations.Single(i => i.Name == nameof(ISignalR).Substring(1)).Token}"),
        };

        var rapidSosOptions = new RefitSettings()
        {
            AuthorizationHeaderValueGetter = (request, cancellationToken) =>
                Task.FromResult($"Bearer {settings.Integrations.Single(i => i.Name == nameof(IRapidSos).Substring(1)).Token}"),
        };

        var twilioOptions = new RefitSettings()
        {
            AuthorizationHeaderValueGetter = (request, cancellationToken) =>
                Task.FromResult($"Bearer {settings.Integrations.Single(i => i.Name == nameof(ITwilio).Substring(1)).Token}"),
        };


        builder.Services.AddRefitClient<IOneSignal>(rapidSosOptions)
                        .ConfigureHttpClient(c => c.BaseAddress = new Uri(settings.Integrations.Single(i => i.Name == nameof(IRapidSos).Substring(1)).EndPoint))
                        .AddPolicyHandler(retryPolicy)
                        .AddPolicyHandler(timeoutPolicy);

        builder.Services.AddRefitClient<IOneSignal>(oneSignalOptions)
                        .ConfigureHttpClient(c => c.BaseAddress = new Uri(settings.Integrations.Single(i => i.Name == nameof(IOneSignal).Substring(1)).EndPoint))
                        .AddPolicyHandler(retryPolicy)
                        .AddPolicyHandler(timeoutPolicy);

        builder.Services.AddRefitClient<ISignalR>(signalROptions)
                        .ConfigureHttpClient(c => c.BaseAddress = new Uri(settings.Integrations.Single(i => i.Name == nameof(ISignalR).Substring(1)).EndPoint))
                        .AddPolicyHandler(retryPolicy)
                        .AddPolicyHandler(timeoutPolicy);

        builder.Services.AddRefitClient<ITwilio>(twilioOptions)
                       .ConfigureHttpClient(c => c.BaseAddress = new Uri(settings.Integrations.Single(i => i.Name == nameof(ITwilio).Substring(1)).EndPoint))
                       .AddPolicyHandler(retryPolicy)
                       .AddPolicyHandler(timeoutPolicy);

    }

    public static IServiceCollection AddPresentationLayer(this IServiceCollection services, IWebHostEnvironment environment)
    {
            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
                options.RespectBrowserAcceptHeader = true;
            })
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                        options.JsonSerializerOptions.IgnoreReadOnlyFields = true;
                        options.JsonSerializerOptions.PropertyNamingPolicy = new LowerCaseNamingPolicy();
                    });

        return services;
    }

    public static void AddAuthorization(this WebApplicationBuilder builder, AppSettings settings)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = $"https://{settings.AppHostname}",
                ValidAudience = settings.Application,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.UserTokens.Key)),
                ClockSkew = TimeSpan.Zero
            };
        });
    }


    private static IServiceCollection AddSwaggerVersioning(this IServiceCollection services, string environment, string application)
    {
        var basePath = AppContext.BaseDirectory;
        var apiComments = Path.Combine(basePath, $"VGStandard.Services.{application}.xml");
        var applicationComments = Path.Combine(basePath, "VGStandard.Application.xml");

        services.AddSwaggerGen(c =>
        {
            c.IncludeXmlComments(apiComments);
            c.IncludeXmlComments(applicationComments);
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Jwt Authorization",
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
                            Id= "Bearer"
                        }
                    },
                    new string[]{}
                    }
                });
        });
        services.ConfigureOptions<ConfigureSwaggerOptions>();
        return services;
    }

    public static IServiceCollection AddCacheService(this IServiceCollection services, AppSettings.CacheSetting cacheSettings)
    {
        if (cacheSettings.CacheType == "Memory")
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
        }
        else if (cacheSettings.CacheType == "Redis")
        {
            services.AddSingleton<ICacheService, RedisCacheService>();
        }
        else
        {
            services.AddSingleton<ICacheService, NoCacheService>();
        }

        return services;
    }
}
