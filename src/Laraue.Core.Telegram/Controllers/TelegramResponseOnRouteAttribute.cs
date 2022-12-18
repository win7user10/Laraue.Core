namespace Laraue.Core.Telegram.Controllers;

[AttributeUsage(AttributeTargets.Method)]
public class TelegramResponseOnRouteAttribute : TelegramBaseRouteAttribute
{
    public TelegramResponseOnRouteAttribute(string pathPattern)
        : base(pathPattern)
    {
    }
}