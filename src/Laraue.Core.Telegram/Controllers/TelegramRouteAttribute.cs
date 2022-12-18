namespace Laraue.Core.Telegram.Controllers;

[AttributeUsage(AttributeTargets.Method)]
public class TelegramRouteAttribute : TelegramBaseRouteAttribute
{
    public TelegramRouteAttribute(string pathPattern)
        : base(pathPattern)
    {
    }
}