using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laraue.Core.Telegram.Authentication;
using Laraue.Core.Telegram.Router.Routes;
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
    
    public async Task<object?> RouteAsync(Update update, CancellationToken cancellationToken = default)
    {
        foreach (var route in _routes)
        {
            if (!route.TryMatch(update, out var pathParams))
            {
                continue;
            }
            
            var from = update.Message?.From
                ?? update.CallbackQuery?.From;
                
            if (!RegisteredUsers.TryGetValue(@from.Id, out var systemId))
            {
                var result = await _userService.LoginOrRegisterAsync(
                    new TelegramData(@from.Id, @from.Username));
                RegisteredUsers.TryAdd(@from.Id, result.UserId);
                systemId = result.UserId;
            }
            
            var request = route.GetRequest(update, pathParams, systemId, cancellationToken);

            return await _mediator.Send((object) request, cancellationToken);
        }

        return null;
    }
}