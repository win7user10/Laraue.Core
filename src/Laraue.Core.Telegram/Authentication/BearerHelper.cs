using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Laraue.Core.Telegram.Authentication;

public static class BearerHelper
{
    public static string GenerateAccessToken(string id, string userName, AuthenticationOptions options)
    {
        var claims = new[]
        {
            new Claim(options.LoginClaimName, userName),
            new Claim(options.IdentifierClaimName, id),
        };

        var token = new JwtSecurityToken(
            options.IssuerName,
            claims: claims,
            signingCredentials: new SigningCredentials(options.SecretKey, options.SigningAlgorithm)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}