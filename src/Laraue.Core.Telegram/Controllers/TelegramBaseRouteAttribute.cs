using Telegram.Bot.Types;

namespace Laraue.Core.Telegram.Controllers;

/// <summary>
/// Attribute that describe matching of telegram request with controller method.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public abstract class TelegramBaseRouteAttribute : Attribute
{
    /// <summary>
    /// This method should return does 
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    public abstract bool IsMatch(Update update);
}