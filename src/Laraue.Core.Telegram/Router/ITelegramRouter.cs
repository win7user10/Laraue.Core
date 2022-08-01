using Telegram.Bot.Types;

namespace Laraue.Core.Telegram.Router;

public interface ITelegramRouter
{
    /// <summary>
    /// Handle telegram update and return something as response for message.
    /// </summary>
    /// <param name="update"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<object?> RouteAsync(Update update, CancellationToken cancellationToken = default);
}