using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Laraue.Core.Telegram.Controllers;

/// <summary>
/// Attribute that describe matching of telegram request with controller method
/// based on the <see cref="UpdateType"/> of <see cref="Update"/> request and the
/// path of the request. 
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public abstract class TelegramBaseRouteWithPathAttribute : TelegramBaseRouteAttribute
{
    /// <summary>
    /// Telegram update type.
    /// </summary>
    public UpdateType UpdateType { get; }
    
    /// <summary>
    /// Pattern of path which will be tried to match.
    /// </summary>
    public string PathPattern { get; } // TODO - Replace to regex and rewrite is match

    protected TelegramBaseRouteWithPathAttribute(UpdateType updateType, string pathPattern)
    {
        UpdateType = updateType;
        PathPattern = pathPattern;
    }

    public override bool IsMatch(Update update)
    {
        return update.Type == UpdateType && GetPathFromUpdate(update) == PathPattern;
    }

    /// <summary>
    /// Returns path identifier from the <see cref="Update"/>.
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    protected abstract string? GetPathFromUpdate(Update update);
}