using System.Reflection;
using Laraue.Core.Telegram.Authentication;
using Laraue.Core.Telegram.Controllers;
using Laraue.Core.Telegram.Router;
using Laraue.Core.Telegram.Router.Middleware;
using Laraue.Core.Telegram.Router.Routing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Telegram.Bot;
using AuthenticationOptions = Laraue.Core.Telegram.Authentication.AuthenticationOptions;

namespace Laraue.Core.Telegram.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds to the container telegram controllers.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="telegramBotClientOptions"></param>
    /// <param name="controllerAssemblies"></param>
    /// <returns></returns>
    public static IServiceCollection AddTelegramCore(
        this IServiceCollection serviceCollection,
        TelegramBotClientOptions telegramBotClientOptions,
        Assembly[]? controllerAssemblies = null)
    {
        serviceCollection.AddScoped<WebApplicationExtensions.MapRequestToTelegramCoreMiddleware>();
        
        serviceCollection
            .AddSingleton(telegramBotClientOptions)
            .AddSingleton<ITelegramBotClient, TelegramBotClient>()
            .AddScoped<ITelegramRouter, TelegramRouter>()
            .AddScoped<TelegramRequestContext>();

        serviceCollection.AddTelegramControllers(controllerAssemblies ?? new []{ Assembly.GetCallingAssembly() });
        serviceCollection.AddOptions<MiddlewareList>();
        serviceCollection.Configure<MiddlewareList>(opt =>
        {
            opt.AddToRoot<ExecuteRouteMiddleware>();
            opt.AddToTop<HandleExceptionsMiddleware>();
        });

        return serviceCollection;
    }

    /// <summary>
    /// Add authentication middleware to the container and configure identity.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <typeparam name="TUser"></typeparam>
    /// <returns></returns>
    public static IdentityBuilder AddTelegramAuthentication<TUser>(
        this IServiceCollection serviceCollection)
        where TUser : TelegramIdentityUser, new()
    {
        serviceCollection.AddTelegramMiddleware<AuthTelegramMiddleware>();
        serviceCollection.ConfigureOptions<ConfigureJwtBearerOptions>();
        
        serviceCollection.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme =  JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        serviceCollection.AddScoped<IUserService, UserService<TUser>>();
        serviceCollection.AddSingleton<UserEvents>();
        
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

    /// <summary>
    /// Allows to add a custom middleware to the telegram request pipeline.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <typeparam name="TMiddleware"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddTelegramMiddleware<TMiddleware>(this IServiceCollection serviceCollection)
        where TMiddleware : class, ITelegramMiddleware
    {
        serviceCollection.Configure<MiddlewareList>(middlewareList =>
        {
            middlewareList.Add<TMiddleware>();
        });

        return serviceCollection;
    }

    /// <summary>
    /// Add opportunity to "answer" on the routes.
    /// The next schema can be used:
    /// 1. Message route executes and asks something from the client. Ex: "What is your age?"
    /// 2. Message route should register response awaiting for the question.
    /// _responseAwaiter.RegisterAwaiter(IResponseAwaiter awaiter)
    /// 3. Next response will try to find registered awaiter and only if it was not found
    /// will execute usual routing, otherwise execute that awaiter.
    /// Should be registered before authentication middleware.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="storageLifetime"></param>
    /// <param name="awaiterAssemblies"></param>
    /// <typeparam name="TAwaiterStorage"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddRouteResponseFunctionality<TAwaiterStorage>(
        this IServiceCollection serviceCollection,
        ServiceLifetime storageLifetime = ServiceLifetime.Scoped,
        IEnumerable<Assembly>? awaiterAssemblies = null)
        where TAwaiterStorage : class, IResponseAwaiterStorage
    {
        var responseAwaiters = (awaiterAssemblies ?? new []{ Assembly.GetCallingAssembly() })
            .SelectMany(x => x.GetTypes())
            .Where(x => x is { IsClass: true, IsAbstract: false } && x.IsAssignableTo(typeof(IResponseAwaiter)));

        foreach (var responseAwaiter in responseAwaiters)
        {
            serviceCollection.AddScoped(responseAwaiter);
        }
        
        serviceCollection.AddTelegramMiddleware<ResponseAwaiterMiddleware>()
            .Add(new ServiceDescriptor(typeof(IResponseAwaiterStorage), typeof(TAwaiterStorage), storageLifetime));

        return serviceCollection;
    }

    private static void AddTelegramControllers(this IServiceCollection serviceCollection, IEnumerable<Assembly> controllerAssemblies)
    {
        var controllerTypes = controllerAssemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => x is { IsClass: true, IsAbstract: false } && x.IsSubclassOf(typeof(TelegramController)));

        foreach (var controllerType in controllerTypes)
        {
            serviceCollection.AddScoped(controllerType);
            
            var methodInfos = controllerType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => x.GetCustomAttribute<TelegramBaseRouteAttribute>(true) != null);
            
            foreach (var methodInfo in methodInfos)
            {
                var routeAttribute = methodInfo.GetCustomAttribute<TelegramBaseRouteAttribute>(true);
                if (routeAttribute is null)
                {
                    continue;
                }
                
                serviceCollection.AddSingleton<IRoute>(new Route(routeAttribute.IsMatch, methodInfo));
            }
        }
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