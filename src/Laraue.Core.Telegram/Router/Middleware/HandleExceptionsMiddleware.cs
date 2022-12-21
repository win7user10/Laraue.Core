using Laraue.Core.Telegram.Exceptions;
using Laraue.Core.Telegram.Extensions;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Laraue.Core.Telegram.Router.Middleware;

public class HandleExceptionsMiddleware : ITelegramMiddleware
{
    private readonly ITelegramMiddleware _next;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ILogger<HandleExceptionsMiddleware> _logger;

    public HandleExceptionsMiddleware(
        ITelegramMiddleware next,
        ITelegramBotClient telegramBotClient,
        ILogger<HandleExceptionsMiddleware> logger)
    {
        _next = next;
        _telegramBotClient = telegramBotClient;
        _logger = logger;
    }
    
    public async Task<object?> InvokeAsync(TelegramRequestContext context, CancellationToken ct = default)
    {
        try
        {
            return await _next.InvokeAsync(context, ct);
        }
        catch (BadTelegramRequestException ex)
        {
            var userId = context.Update.GetUserId();
            
            await _telegramBotClient.SendTextMessageAsync(
                userId,
                ex.Message,
                cancellationToken: ct);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Telegram request handling error");
        }

        return null;
    }
}