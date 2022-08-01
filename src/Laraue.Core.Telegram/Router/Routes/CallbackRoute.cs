using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Laraue.Core.Telegram.Router.Routes;

public class CallbackRoute : BaseRoute<CallbackQuery>
{
    public CallbackRoute(string routePattern, PerformRoute<CallbackQuery> getRequest)
        : base(UpdateType.CallbackQuery, routePattern, getRequest)
    {
    }

    protected override string? GetContent(CallbackQuery data)
    {
        return data.Data;
    }

    protected override CallbackQuery GetEntity(Update update)
    {
        return update.CallbackQuery!;
    }
}