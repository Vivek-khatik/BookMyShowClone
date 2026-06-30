using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BookMyShow.Application.Interfaces;
using BookMyShow.Domain.Entities.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BookMyShow.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(User user, IReadOnlyCollection<string> roles)
    {
        var key = GetSecurityKey();
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: GetRequiredJwtValue("Issuer"),
            audience: GetRequiredJwtValue("Audience"),
            claims: claims,
            expires: GetAccessTokenExpiresAt(),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }

    public DateTime GetAccessTokenExpiresAt()
    {
        var configuredMinutes = _configuration["Jwt:AccessTokenExpirationMinutes"];
        var minutes = int.TryParse(configuredMinutes, out var parsedMinutes) ? parsedMinutes : 30;

        return DateTime.UtcNow.AddMinutes(minutes);
    }

    private SymmetricSecurityKey GetSecurityKey()
    {
        var key = GetRequiredJwtValue("Key");
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }

    private string GetRequiredJwtValue(string name)
    {
        var value = _configuration[$"Jwt:{name}"];

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Jwt:{name} is not configured.");
        }

        return value;
    }
}
