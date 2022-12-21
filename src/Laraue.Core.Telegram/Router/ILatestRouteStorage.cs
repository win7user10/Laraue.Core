namespace Laraue.Core.Telegram.Router;

/// <summary>
/// Latest route info to understand telegram user context.
/// </summary>
public interface ILatestRouteStorage
{
    Task<string?> GetLatestRouteAsync(string userId);
    Task SetLatestRouteAsync(string userId, string? latestRoute);
}