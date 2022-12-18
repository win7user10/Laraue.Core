using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Laraue.Core.Telegram.Router.Routes;

public class MessageRoute : BaseRoute<Message>
{
    public MessageRoute(string routePattern, Type routeAttributeType, ExecuteRouteAsync<Message> executeRouteAsyncDelegate)
        : base(UpdateType.Message, routePattern, routeAttributeType, executeRouteAsyncDelegate)
    {
    }

    protected override string? GetContent(Message data)
    {
        return data.Text;
    }

    protected override Message GetEntity(Update update)
    {
        return update.Message!;
    }
}