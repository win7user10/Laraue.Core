using System.Security.Claims;
using System.Text.Encodings.Web;
using Laraue.Core.Telegram.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laraue.Core.Telegram.Authentication;

public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationOptions>
{
    public CustomAuthenticationHandler(
        IOptionsMonitor<AuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) 
        : base(options, logger, encoder, clock)
    {
    }

    /// <summary>
    /// TODO - finish this method.
    /// </summary>
    /// <returns></returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (true)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        return Task.FromResult(
            AuthenticateResult.Success(
                new AuthenticationTicket(
                    new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new Claim[]
                            {
                                new ("id", "user_identifier")
                            }, Options.AuthScheme)), Options.AuthScheme)));
    }
}

