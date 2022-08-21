using System.Collections.Concurrent;
using Laraue.Core.Telegram.Authentication;
using Laraue.Core.Telegram.Extensions;
using Laraue.Core.Telegram.Router.Routes;
using Laraue.Core.Threading;
using MediatR;
using Telegram.Bot.Types;

namespace Laraue.Core.Telegram.Router;

public class TelegramRouter : ITelegramRouter
{
    private readonly IEnumerable<IRoute> _routes;
    private readonly IMediator _mediator;
    private readonly IUserService _userService;

    public TelegramRouter(IMediator mediator, IUserService userService, IEnumerable<IRoute> routes)
    {
        _mediator = mediator;
        _userService = userService;
        _routes = routes;
    }
    
    private static readonly ConcurrentDictionary<long, string> RegisteredUsers = new ();
    private static readonly KeyedSemaphoreSlim<long> Semaphore = new (1, 1);
    
    public virtual async Task<object?> RouteAsync(Update update, CancellationToken cancellationToken = default)
    {
        using var _ = await Semaphore.WaitAsync(update.Id, cancellationToken);
        
        foreach (var route in _routes)
        {
            if (!route.TryMatch(update, out var pathParams))
            {
                continue;
            }
            
            var from = update.GetUser()!;
            
            if (!RegisteredUsers.TryGetValue(from.Id, out var systemId))
            {
                var result = await _userService.LoginOrRegisterAsync(
                    new TelegramData(from.Id, from.Username));
                RegisteredUsers.TryAdd(from.Id, result.UserId);
                systemId = result.UserId;
            }
            
            var request = route.GetRequest(update, pathParams, systemId);

            return await _mediator.Send((object) request, cancellationToken);
        }

        return null;
    }
}