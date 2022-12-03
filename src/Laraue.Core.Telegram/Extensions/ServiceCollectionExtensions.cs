using Laraue.Core.Telegram.Authentication;
using Laraue.Core.Telegram.Router;
using Laraue.Core.Telegram.Router.Routes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Telegram.Bot;
using AuthenticationOptions = Laraue.Core.Telegram.Authentication.AuthenticationOptions;

namespace Laraue.Core.Telegram.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds to the container telegram dependencies.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="routes"></param>
    /// <typeparam name="TRouter"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    /// <returns></returns>
    public static IdentityBuilder AddTelegramDependencies<TRouter, TUser>(
        this IServiceCollection serviceCollection,
        IEnumerable<IRoute> routes)
        where TRouter : class, ITelegramRouter
        where TUser : TelegramIdentityUser, new()
    {
        serviceCollection
            .AddSingleton<ITelegramBotClient, TelegramBotClient>()
            .AddScoped<ITelegramRouter, TRouter>()
            .AddSingleton(routes);
        
        serviceCollection.ConfigureOptions<ConfigureJwtBearerOptions>();

        serviceCollection.AddScoped<IUserService, UserService<TUser>>();
        
        serviceCollection.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme =  JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        });
        
        return serviceCollection.AddIdentity<TUser, IdentityRole>(opt =>
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