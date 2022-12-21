namespace Laraue.Core.Telegram.Controllers;

[AttributeUsage(AttributeTargets.Method)]
public class TelegramResponseOfRouteAttribute : TelegramBaseRouteAttribute
{
    public TelegramResponseOfRouteAttribute(string pathPattern)
        : base(pathPattern)
    {
    }
}