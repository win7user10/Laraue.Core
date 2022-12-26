using Laraue.Core.Telegram.Controllers;
using Laraue.Core.Telegram.Router.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Core.Telegram.Router.Middleware;

public class ResponseAwaiterMiddleware : ITelegramMiddleware
{
    private readonly ITelegramMiddleware _next;
    private readonly IResponseAwaiterStorage _responseAwaiterStorage;
    private readonly TelegramRequestContext _requestContext;
    private readonly IServiceProvider _serviceProvider;

    public ResponseAwaiterMiddleware(
        ITelegramMiddleware next,
        IResponseAwaiterStorage responseAwaiterStorage,
        TelegramRequestContext requestContext,
        IServiceProvider serviceProvider)
    {
        _next = next;
        _responseAwaiterStorage = responseAwaiterStorage;
        _requestContext = requestContext;
        _serviceProvider = serviceProvider;
    }
    
    public async Task<object?> InvokeAsync(CancellationToken ct = default)
    {
        if (_requestContext.UserId is null)
        {
            throw new InvalidOperationException(
                $"User information is not available." +
                $" Ensure {typeof(AuthTelegramMiddleware)} has been registered");
        }
        
        var responseAwaiterType = await _responseAwaiterStorage.TryGetAsync(_requestContext.UserId!);

        if (responseAwaiterType is null)
        {
            return await _next.InvokeAsync(ct);
        }
        
        var responseAwaiter = (_serviceProvider.GetRequiredService(responseAwaiterType) as IResponseAwaiter)!;
        var result = await responseAwaiter.ExecuteAsync(_serviceProvider);
        
        await _responseAwaiterStorage.ResetAsync(_requestContext.UserId);
        
        return result;
    }
}