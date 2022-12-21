namespace Laraue.Core.Telegram.Router.Middleware;

/// <summary>
/// Additional logic to execute before/after request.
/// </summary>
public interface ITelegramMiddleware
{
    public Task<object?> InvokeAsync(TelegramRequestContext context, CancellationToken ct = default);
}