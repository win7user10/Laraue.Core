using System.Collections.Concurrent;
using Laraue.Core.Telegram.Authentication;
using Laraue.Core.Telegram.Extensions;
using Laraue.Core.Threading;
using Microsoft.Extensions.Logging;

namespace Laraue.Core.Telegram.Router.Middleware;

public class AuthTelegramMiddleware : ITelegramMiddleware
{
    private readonly ITelegramMiddleware _next;
    private readonly IUserService _userService;
    private readonly TelegramRequestContext _telegramRequestContext;
    private readonly ILogger<AuthTelegramMiddleware> _logger;

    private static readonly ConcurrentDictionary<long, string> UserIdTelegramIdMap = new ();
    private static readonly KeyedSemaphoreSlim<long> Semaphore = new (1, 1);

    public AuthTelegramMiddleware(
        ITelegramMiddleware next,
        IUserService userService,
        TelegramRequestContext telegramRequestContext,
        ILogger<AuthTelegramMiddleware> logger)
    {
        _next = next;
        _userService = userService;
        _telegramRequestContext = telegramRequestContext;
        _logger = logger;
    }
    
    public async Task<object?> InvokeAsync(CancellationToken ct = default)
    {
        var from = _telegramRequestContext.Update.GetUser()!;
        
        using var _ = await Semaphore.WaitAsync(from.Id, ct);

        if (!UserIdTelegramIdMap.TryGetValue(from.Id, out var systemId))
        {
            var result = await _userService.LoginOrRegisterAsync(
                new TelegramData(from.Id, from.Username!));
            
            UserIdTelegramIdMap.TryAdd(from.Id, result.UserId);
            systemId = result.UserId;
        }
        
        _telegramRequestContext.UserId = systemId;
        
        _logger.LogInformation("Auth as: {TelegramId}, {SystemId}", from.Id, systemId);
        return await _next.InvokeAsync(ct);
    }
}