using System;
using System.Collections.Generic;
using System.Text;

namespace BookMyShow.Application.DTOs.Auth
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = null!;
    }
}
