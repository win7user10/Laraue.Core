using Laraue.Core.Telegram.Controllers;
using Laraue.Core.Telegram.Router.Routes;

namespace Laraue.Core.Telegram.Router.Middleware;

public class LatestRouteMiddleware : ITelegramMiddleware
{
    private readonly ITelegramMiddleware _next;
    private readonly IEnumerable<IRoute> _routes;
    private readonly ILatestRouteStorage _latestRouteStorage;

    public LatestRouteMiddleware(
        ITelegramMiddleware next,
        IEnumerable<IRoute> routes,
        ILatestRouteStorage latestRouteStorage)
    {
        _next = next;
        _routes = routes;
        _latestRouteStorage = latestRouteStorage;
    }
    
    public async Task<object?> InvokeAsync(TelegramRequestContext context, CancellationToken ct = default)
    {
        if (context.UserId is null)
        {
            throw new InvalidOperationException("User information is not available");
        }
        
        var latestRoute = await _latestRouteStorage.GetLatestRouteAsync(context.UserId!);

        if (latestRoute is not null)
        {
            foreach (var route in _routes)
            {
                if (!route.TryMatch(context.Update.Type, latestRoute, typeof(TelegramResponseOnRouteAttribute)))
                {
                    continue;
                }
                
                var lastRouteResult = await route.ExecuteAsync(context.Update, context.UserId!);
                context.ExecutedRoute = null;
                
                await _latestRouteStorage.SetLatestRouteAsync(context.UserId, context.ExecutedRoute);

                return lastRouteResult;
            }
        }

        var result = await _next.InvokeAsync(context, ct);
        
        await _latestRouteStorage.SetLatestRouteAsync(context.UserId, context.ExecutedRoute);

        return result;
    }
}