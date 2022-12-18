namespace Laraue.Core.Telegram.Router.Middleware;

public sealed class MiddlewareList
{
    public Type RootMiddleware { get; internal set; }

    public Queue<Type> MiddlewareTypes = new ();
}