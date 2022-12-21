using Laraue.Core.Telegram.Router.Request;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Laraue.Core.Telegram.Router.Routes;

/// <summary>
/// One of the telegram routes.
/// </summary>
public interface IRoute
{
    string Pattern { get; }

    /// <summary>
    /// Check does this route can handle telegram <see cref="Update"/>.
    /// </summary>
    /// <param name="updateType"></param>
    /// <param name="route"></param>
    /// <param name="routeAttributeType"></param>
    /// <returns></returns>
    bool TryMatch(UpdateType updateType, string? route, Type routeAttributeType);
    
    /// <summary>
    /// Execute passed update with current route.
    /// </summary>
    /// <param name="update"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<object?> ExecuteAsync(Update update, string userId);

    string? GetContent(Update update);
}

/// <summary>
/// Route delegate.
/// </summary>
/// <typeparam name="T"></typeparam>
public delegate Task<object?> ExecuteRouteAsync<T>(RouteData<T> routeData);

/// <summary>
/// Route context.
/// </summary>
/// <param name="Data"></param>
/// <param name="Context"></param>
/// <param name="Parameters"></param>
/// <param name="PathParameters"></param>
/// <typeparam name="T"></typeparam>
public record RouteData<T>(T Data, RequestContext Context, RequestParameters Parameters, PathParameters PathParameters);