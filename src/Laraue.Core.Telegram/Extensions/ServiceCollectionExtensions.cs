using Laraue.Core.Telegram.Authentication;
using Laraue.Core.Telegram.Router;
using Laraue.Core.Telegram.Router.Routes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Telegram.Bot;
using AuthenticationOptions = Laraue.Core.Telegram.Authentication.AuthenticationOptions;

namespace Laraue.Core.Telegram.Extensions;

public static class ServiceCollectionExtensions
{
    public static AuthenticationBuilder AddJwtTelegramAuthentication(this IServiceCollection serviceCollection)
    {
        serviceCollection.ConfigureOptions<ConfigureJwtBearerOptions>();

        serviceCollection.AddScoped<IUserService, UserService>();
        
        return serviceCollection.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        });
    }

    public static IServiceCollection AddSwaggerGeneration(this IServiceCollection serviceCollection)
    {
        serviceCollection.ConfigureOptions<ConfigureSwaggerGenOptions>();
        
        return serviceCollection.AddSwaggerGen();
    }

    public static IdentityBuilder AddTelegramDependencies(
        this IServiceCollection serviceCollection,
        IEnumerable<IRoute> routes)
    {
        serviceCollection
            .AddSingleton<IMemoryCache, MemoryCache>()
            .AddSingleton<ITelegramBotClient, TelegramBotClient>()
            .AddScoped<ITelegramRouter, TelegramRouter>()
            .AddSingleton(routes);
        
        return serviceCollection.AddIdentity<TelegramIdentityUser, IdentityRole>(opt =>
        {
            opt.Password.RequireDigit = false;
            opt.Password.RequiredLength = 1;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequiredUniqueChars = 1;
        });
    }
}

public class ConfigureSwaggerGenOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly AuthenticationOptions _options;

    public ConfigureSwaggerGenOptions(IOptionsMonitor<AuthenticationOptions> options)
    {
        _options = options.CurrentValue;
    }

    public void Configure(SwaggerGenOptions options)
    {
        options.AddSecurityDefinition(_options.TokenHeaderName, new OpenApiSecurityScheme
        {
            Name = _options.TokenHeaderName,
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = _options.AuthScheme,
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = _options.TokenHeaderName
                    },
                    Scheme = _options.AuthScheme,
                    Name = _options.TokenHeaderName,
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });
    }

    public void Configure(string name, SwaggerGenOptions options)
    {
        Configure(options);
    }
}

public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly AuthenticationOptions _options;

    public ConfigureJwtBearerOptions(IOptionsMonitor<AuthenticationOptions> options)
    {
        _options = options.CurrentValue;
    }

    public void Configure(JwtBearerOptions options)
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidIssuer = _options.IssuerName,
            ValidateAudience = false,
            IssuerSigningKey = _options.SecretKey,
            ValidateLifetime = false,
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                context.Token = accessToken;
                return Task.CompletedTask;
            }
        };
    }

    public void Configure(string name, JwtBearerOptions options)
    {
        Configure(options);
    }
}