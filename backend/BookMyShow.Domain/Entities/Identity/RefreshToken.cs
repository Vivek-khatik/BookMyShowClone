using BookMyShow.Domain.Common;

namespace BookMyShow.Domain.Entities.Identity;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }
}
