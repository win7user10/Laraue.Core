using System.Diagnostics.CodeAnalysis;
using Laraue.Core.Telegram.Router.Request;
using MediatR;
using Telegram.Bot.Types;

namespace Laraue.Core.Telegram.Router.Routes;

/// <summary>
/// One of the telegram routes.
/// </summary>
public interface IRoute
{
    /// <summary>
    /// Check does this route can handle telegram <see cref="Update"/>.
    /// </summary>
    /// <param name="update"></param>
    /// <param name="pathParameters"></param>
    /// <returns></returns>
    bool TryMatch(Update update, [NotNullWhen(true)] out PathParameters? pathParameters);
    
    /// <summary>
    /// Get mediator command to handle update.
    /// </summary>
    /// <param name="update"></param>
    /// <param name="pathParameters"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    IRequest<object?> GetRequest(
        Update update,
        PathParameters pathParameters,
        string userId);
}

/// <summary>
/// Route delegate.
/// </summary>
/// <typeparam name="T"></typeparam>
public delegate IRequest<object?> PerformRoute<T>(RouteData<T> routeData);

/// <summary>
/// Route context.
/// </summary>
/// <param name="Data"></param>
/// <param name="Context"></param>
/// <param name="Parameters"></param>
/// <param name="PathParameters"></param>
/// <typeparam name="T"></typeparam>
public record RouteData<T>(T Data, RequestContext Context, RequestParameters Parameters, PathParameters PathParameters);