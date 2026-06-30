using BookMyShow.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookMyShow.Application.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest request);

        Task<LoginResponse> LoginAsync(LoginRequest request);

        Task<LoginResponse> RefreshTokenAsync(string refreshToken);

        Task LogoutAsync(string refreshToken);
    }
}
