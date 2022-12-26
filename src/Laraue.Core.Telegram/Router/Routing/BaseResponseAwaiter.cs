using System.Diagnostics.CodeAnalysis;
using Laraue.Core.Telegram.Exceptions;
using Laraue.Core.Telegram.Router.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace Laraue.Core.Telegram.Router.Routing;

public abstract class BaseResponseAwaiter : IResponseAwaiter
{
    public async Task<object?> ExecuteAsync(IServiceProvider serviceProvider)
    {
        var telegramRequestContext = serviceProvider.GetRequiredService<TelegramRequestContext>();

        if (!TryValidate(telegramRequestContext.Update, out var error))
        {
            throw new BadTelegramRequestException(error);
        }

        return await ExecuteRouteAsync(telegramRequestContext);
    }

    protected abstract bool TryValidate(Update update, [NotNullWhen(false)] out string? errorMessage);
    
    protected abstract Task<object?> ExecuteRouteAsync(TelegramRequestContext telegramRequestContext);
}