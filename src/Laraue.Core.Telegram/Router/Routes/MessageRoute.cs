using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Laraue.Core.Telegram.Router.Routes;

public class MessageRoute : BaseRoute<Message>
{
    public MessageRoute(string routePattern, PerformRoute<Message> getRequest)
        : base(UpdateType.Message, routePattern, getRequest)
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