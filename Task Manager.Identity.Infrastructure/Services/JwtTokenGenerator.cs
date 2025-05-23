using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Task_Manager.Identity.Application.Services.Abstractions;
using Task_Manager.Identity.Application.Services.Contracts;
using Task_Manager.Identity.Core.Entities;
using Task_Manager.Integrations;

namespace Task_Manager.Identity.Infrastructure.Services;

public class JwtTokenGenerator(
    IOptions<JwtOptions> options,
    TimeProvider timeProvider
) : IJwtTokenGenerator
{
    private readonly JwtOptions _options = options.Value;
    private readonly TimeProvider _timeProvider = timeProvider;

    public JwtToken GenerateToken(ApplicationUser user)
    {
        var issuedAt = _timeProvider.GetUtcNow();

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.DisplayName),
            new Claim(JwtRegisteredClaimNames.Iat, issuedAt.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: issuedAt.UtcDateTime,
            expires: issuedAt.Add(_options.AccessTokenLifetime).UtcDateTime,
            signingCredentials: creds
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new JwtToken(
            accessToken,
            "", // TODO: Implement refresh token generation
            issuedAt,
            _options.AccessTokenLifetime
        );
    }
}
