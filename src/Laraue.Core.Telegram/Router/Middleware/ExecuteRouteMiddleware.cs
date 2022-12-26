using Laraue.Core.Telegram.Router.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Core.Telegram.Router.Middleware;

internal sealed class ExecuteRouteMiddleware : ITelegramMiddleware
{
    private readonly IEnumerable<IRoute> _routes;
    private readonly IServiceProvider _serviceProvider;

    public ExecuteRouteMiddleware(IEnumerable<IRoute> routes, IServiceProvider serviceProvider)
    {
        _routes = routes;
        _serviceProvider = serviceProvider;
    }

    public async Task<object?> InvokeAsync(CancellationToken ct = default)
    {
        foreach (var route in _routes)
        {
            var result = await route.TryExecuteAsync(_serviceProvider);
            
            if (!result.IsExecuted)
            {
                continue;
            }

            return result.ExecutionResult;
        }

        return null;
    }
}