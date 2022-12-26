using Laraue.Core.Telegram.Router.Routing;

namespace Laraue.Core.Telegram.Router;

/// <summary>
/// Latest route info to understand telegram user context.
/// </summary>
public interface IResponseAwaiterStorage
{
    Task<Type?> TryGetAsync(string userId);
    Task SetAsync<TResponseAwaiter>(string userId) where TResponseAwaiter : IResponseAwaiter;
    Task ResetAsync(string userId);
}