using System.Diagnostics;
using Laraue.Core.Telegram.Router.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Laraue.Core.Telegram.Router;

public sealed class TelegramRouter : ITelegramRouter
{
    private readonly MiddlewareList _middlewareList;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelegramRouter> _logger;

    public TelegramRouter(
        IServiceProvider serviceProvider,
        IOptions<MiddlewareList> middlewareList,
        ILogger<TelegramRouter> logger)
    {
        _middlewareList = middlewareList.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<object?> RouteAsync(Update update, CancellationToken cancellationToken = default)
    {
        var sw = new Stopwatch();
        sw.Start();

        ITelegramMiddleware? lastMiddleware = null;
        foreach (var middlewareType in _middlewareList.Items)
        {
            var middleware = lastMiddleware == null
                ? ActivatorUtilities.CreateInstance(_serviceProvider, middlewareType)
                : ActivatorUtilities.CreateInstance(_serviceProvider, middlewareType, lastMiddleware);

            lastMiddleware = (ITelegramMiddleware) middleware;
        }

        var requestContext = new TelegramRequestContext(update);
        var result = await lastMiddleware!.InvokeAsync(
            requestContext,
            cancellationToken);
        
        if (requestContext.ExecutedRoute is not null)
        {
            _logger.LogDebug(
                "Request time {Time} ms, route: {RouteName} executed",
                sw.ElapsedMilliseconds,
                requestContext.ExecutedRoute);
        }
        else
        {
            _logger.LogDebug(
                "Request time {Time} ms, status: not found, payload: {Payload}",
                sw.ElapsedMilliseconds,
                JsonConvert.SerializeObject(update));
        }
        
        return result;
    }
}