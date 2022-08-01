using Laraue.Core.Telegram.Router;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Laraue.Core.Telegram.Extensions;

public static class WebApplicationExtensions
{
    public static void MapTelegramRequests(this WebApplication webApplication, string route)
    {
        webApplication.Map(route, async (HttpContext context, ITelegramRouter router, CancellationToken ct) =>
        {
            using var sr = new StreamReader(context.Request.Body);
            var body = await sr.ReadToEndAsync();
            
            var update = JsonConvert.DeserializeObject<Update>(body);

            return await router.RouteAsync(update, ct);
        });
    }
}