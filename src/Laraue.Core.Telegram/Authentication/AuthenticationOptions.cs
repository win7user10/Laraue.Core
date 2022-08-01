using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Laraue.Core.Telegram.Authentication;

public class AuthenticationOptions : AuthenticationSchemeOptions
{
    public string AuthScheme { get; init; } = "Laraue.Telegram";

    public string SecretPhrase { get; init; } = "This.Value.Should.Be.Overriden";
    
    public string IssuerName { get; init; } = "Laraue";

    public string SigningAlgorithm { get; init; } = SecurityAlgorithms.HmacSha256;

    public string IdentifierClaimName { get; init; } = "uuid";
    
    public string LoginClaimName { get; init; } = "login";

    public string TokenHeaderName = "Token";

    public SymmetricSecurityKey SecretKey => new(Encoding.UTF8.GetBytes(SecretPhrase));
}