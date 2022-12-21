using Laraue.Core.Telegram.Controllers;
using Laraue.Core.Telegram.Router.Routes;

namespace Laraue.Core.Telegram.Router.Middleware;

internal sealed class ExecuteRouteMiddleware : ITelegramMiddleware
{
    private readonly IEnumerable<IRoute> _routes;

    public ExecuteRouteMiddleware(IEnumerable<IRoute> routes)
    {
        _routes = routes;
    }

    public async Task<object?> InvokeAsync(TelegramRequestContext context, CancellationToken ct = default)
    {
        foreach (var route in _routes)
        {
            if (!route.TryMatch(context.Update.Type, route.GetContent(context.Update), typeof(TelegramRouteAttribute)))
            {
                continue;
            }

            var result = await route.ExecuteAsync(context.Update, context.UserId!);
            context.ExecutedRoute = route.Pattern;

            return result;
        }

        return null;
    }
}