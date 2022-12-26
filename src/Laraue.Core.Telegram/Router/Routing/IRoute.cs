namespace Laraue.Core.Telegram.Router.Routing;

/// <summary>
/// One of the telegram routes.
/// </summary>
public interface IRoute
{
    /// <summary>
    /// Try execute route if it is suitable for the execution.
    /// </summary>
    /// <param name="requestProvider"></param>
    /// <returns></returns>
    ValueTask<RouteExecutionResult> TryExecuteAsync(IServiceProvider requestProvider);
}