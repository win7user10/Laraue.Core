using System.Security.Claims;
using Laraue.Core.Telegram.Authentication;
using Microsoft.AspNetCore.Http;

namespace Laraue.Core.Telegram.Extensions;

public static class HttpContextExtensions
{
    public static Claim? GetPlayerIdClaim(this ClaimsPrincipal? user, AuthenticationOptions options)
    {
        return user?
            .Claims
            .FirstOrDefault(x => x.Type == options.IdentifierClaimName);
    }
    
    public static bool TryGetPlayerId(this HttpContext context, AuthenticationOptions options, out Guid playerId)
    {
        var playerIdClaim = context.User.GetPlayerIdClaim(options);

        if (playerIdClaim is not null)
        {
            return Guid.TryParse(playerIdClaim.Value, out playerId);
        }
        
        playerId = Guid.Empty;
        return false;
    }
}