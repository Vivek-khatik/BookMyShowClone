using BookMyShow.Application.DTOs.Auth;
using BookMyShow.Application.Interfaces;
using BookMyShow.Domain.Entities.Identity;
using BookMyShow.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BookMyShow.Infrastructure.Services;

public class AuthService : IAuthService
{
    private const string DefaultCustomerRole = "Customer";

    private readonly ApplicationDbContext _dbContext;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthService(
        ApplicationDbContext dbContext,
        IJwtService jwtService,
        IPasswordHasher passwordHasher,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var email = NormalizeEmail(request.Email);
        var phoneNumber = request.PhoneNumber.Trim();

        var emailExists = await _dbContext.Users.AnyAsync(x => x.Email == email);

        if (emailExists)
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var phoneExists = await _dbContext.Users.AnyAsync(x => x.PhoneNumber == phoneNumber);

        if (phoneExists)
        {
            throw new InvalidOperationException("Phone number is already registered.");
        }

        var customerRole = await _dbContext.Roles.SingleAsync(x => x.Name == DefaultCustomerRole);

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = string.IsNullOrWhiteSpace(request.LastName) ? null : request.LastName.Trim(),
            Email = email,
            PhoneNumber = phoneNumber,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Status = 1,
            CreatedAt = DateTime.UtcNow,
            UserRoles = new List<UserRole>
            {
                new()
                {
                    RoleId = customerRole.Id
                }
            }
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var email = NormalizeEmail(request.Email);
        var user = await GetUserWithRolesByEmailAsync(email);

        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var response = await CreateLoginResponseAsync(user);
        await _dbContext.SaveChangesAsync();

        return response;
    }

    public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _dbContext.RefreshTokens
            .Include(x => x.User)
                .ThenInclude(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
            .SingleOrDefaultAsync(x => x.Token == refreshToken);

        if (storedToken is null || storedToken.IsRevoked || storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        storedToken.IsRevoked = true;
        storedToken.UpdatedAt = DateTime.UtcNow;

        var response = await CreateLoginResponseAsync(storedToken.User);
        await _dbContext.SaveChangesAsync();

        return response;
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var storedToken = await _dbContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

        if (storedToken is null)
        {
            return;
        }

        storedToken.IsRevoked = true;
        storedToken.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    private async Task<User?> GetUserWithRolesByEmailAsync(string email)
    {
        return await _dbContext.Users
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .SingleOrDefaultAsync(x => x.Email == email);
    }

    private async Task<LoginResponse> CreateLoginResponseAsync(User user)
    {
        var roles = user.UserRoles.Select(x => x.Role.Name).ToArray();
        var accessTokenExpiresAt = _jwtService.GetAccessTokenExpiresAt();
        var refreshTokenValue = _jwtService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenExpirationDays()),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.RefreshTokens.Add(refreshToken);

        return new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Roles = roles,
            AccessToken = _jwtService.GenerateAccessToken(user, roles),
            RefreshToken = refreshTokenValue,
            AccessTokenExpiresAt = accessTokenExpiresAt
        };
    }

    private int GetRefreshTokenExpirationDays()
    {
        var configuredDays = _configuration["Jwt:RefreshTokenExpirationDays"];
        return int.TryParse(configuredDays, out var parsedDays) ? parsedDays : 7;
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}
