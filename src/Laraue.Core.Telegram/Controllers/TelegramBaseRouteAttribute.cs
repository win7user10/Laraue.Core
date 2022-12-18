namespace Laraue.Core.Telegram.Controllers;

[AttributeUsage(AttributeTargets.Method)]
public class TelegramBaseRouteAttribute : Attribute
{
    public string PathPattern { get; }

    protected TelegramBaseRouteAttribute(string pathPattern)
    {
        PathPattern = pathPattern;
    }
}