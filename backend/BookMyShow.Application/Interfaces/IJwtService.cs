using BookMyShow.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookMyShow.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user, IReadOnlyCollection<string> roles);

        string GenerateRefreshToken();

        DateTime GetAccessTokenExpiresAt();
    }
}
