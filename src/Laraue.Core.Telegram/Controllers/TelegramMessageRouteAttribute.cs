using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Laraue.Core.Telegram.Controllers;

/// <summary>
/// Route based on the <see cref="Update.Message"/> property of <see cref="Update"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TelegramMessageRouteAttribute : TelegramBaseRouteWithPathAttribute
{
    public TelegramMessageRouteAttribute(string pathPattern)
        : base(UpdateType.Message, pathPattern)
    {
    }

    protected override string? GetPathFromUpdate(Update update)
    {
        return update.Message?.Text;
    }
}