namespace Laraue.Core.Telegram.Router.Routing;

public interface IResponseAwaiter
{
    /// <summary>
    /// Try execute response awaiter if it is suitable for the execution.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    Task<object?> ExecuteAsync(IServiceProvider serviceProvider);
}