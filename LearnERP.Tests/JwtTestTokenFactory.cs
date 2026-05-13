using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LearnERP.Api.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace LearnERP.Tests;

public static class JwtTestTokenFactory
{
    public static string CreateToken(string secret, string issuer, string audience, IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            notBefore: DateTime.UtcNow.AddMinutes(-1),
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
