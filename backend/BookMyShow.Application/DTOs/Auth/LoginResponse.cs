using System;
using System.Collections.Generic;
using System.Text;

namespace BookMyShow.Application.DTOs.Auth
{
    public class LoginResponse
    {
        public Guid UserId { get; set; }

        public string Email { get; set; } = null!;

        public IReadOnlyCollection<string> Roles { get; set; } = Array.Empty<string>();

        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;

        public DateTime AccessTokenExpiresAt { get; set; }
    }
}

