using System.Reflection;
using Laraue.Core.Telegram.Authentication;
using Laraue.Core.Telegram.Controllers;
using Laraue.Core.Telegram.Router;
using Laraue.Core.Telegram.Router.Middleware;
using Laraue.Core.Telegram.Router.Request;
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
    /// Adds to the container telegram controllers.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="controllerAssemblies"></param>
    /// <returns></returns>
    public static IServiceCollection AddTelegramCore(
        this IServiceCollection serviceCollection,
        Assembly[]? controllerAssemblies = null)
    {
        serviceCollection
            .AddSingleton<ITelegramBotClient, TelegramBotClient>()
            .AddScoped<ITelegramRouter, TelegramRouter>();

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

    public static IServiceCollection AddLatestRouteFunctionality<TLatestStorage>(this IServiceCollection serviceCollection)
        where TLatestStorage : class, ILatestRouteStorage
    {
        return serviceCollection.AddTelegramMiddleware<LatestRouteMiddleware>()
            .AddScoped<ILatestRouteStorage, TLatestStorage>();
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
                .Where(x => RouteAttributes.Any(y => x.GetCustomAttribute(y) != null));
            
            foreach (var methodInfo in methodInfos)
            {
                foreach (var routeAttributeType in RouteAttributes)
                {
                    var attribute = methodInfo.GetCustomAttribute(routeAttributeType);
                    if (attribute is null)
                    {
                        continue;
                    }
                    
                    var pathPattern = ((TelegramBaseRouteAttribute)attribute).PathPattern;
                    serviceCollection.AddTelegramRoute(serviceProvider =>
                        new MessageRoute(
                            pathPattern,
                            routeAttributeType,
                            routeData => ExecuteRouteAsync(
                                serviceProvider,
                                routeData,
                                methodInfo,
                                controllerType)));
                }
            }
        }
    }
    
    private static Type[] RouteAttributes { get; } = 
    {
        typeof(TelegramRouteAttribute),
        typeof(TelegramResponseOnRouteAttribute),
    };

    private static async Task<object?> ExecuteRouteAsync<TRoute>(
        IServiceProvider sp,
        RouteData<TRoute> routeData,
        MethodInfo methodInfo,
        Type controllerType)
    {
        using var requestScope = sp.CreateScope();
                        
        var result = methodInfo.Invoke(
            requestScope.ServiceProvider.GetRequiredService(controllerType),
            GetRouteParameters(routeData, methodInfo));

        if (result is null)
        {
            return result;
        }
                        
        if (methodInfo.ReturnType == typeof(Task<>) || methodInfo.ReturnType == typeof(ValueTask<>))
        {
            result = await (dynamic) result;
        }

        else if (methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType == typeof(ValueTask))
        {
            await (dynamic) result;

            return null;
        }

        return result;
    }

    private static object[] GetRouteParameters<TRoute>(RouteData<TRoute> routeData, MethodBase methodInfo)
    {
        var methodParameters = methodInfo.GetParameters();
        var parameters = new object[methodParameters.Length];

        for (var i = 0; i < methodParameters.Length; i++)
        {
            var methodParameter = methodParameters[i];
            if (methodParameter.ParameterType == typeof(TRoute))
            {
                parameters[i] = routeData.Data!;
            }
            else if (methodParameter.ParameterType == typeof(RequestContext))
            {
                parameters[i] = routeData.Context;
            }
            else
            {
                throw new InvalidOperationException(
                    $"Unable to resolve parameter of type {methodParameter.ParameterType}");
            }
        }

        return parameters;
    }

    public static void AddTelegramRoute(this IServiceCollection serviceCollection, IRoute route)
    {
        serviceCollection.AddSingleton(route);
    }
    
    public static void AddTelegramRoute(
        this IServiceCollection serviceCollection,
        Func<IServiceProvider, IRoute> addRoute)
    {
        serviceCollection.AddSingleton(addRoute);
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