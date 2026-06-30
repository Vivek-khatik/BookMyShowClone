using BookMyShow.Domain.Common;

namespace BookMyShow.Domain.Entities.Identity;

public class User : BaseEntity
{
    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool IsEmailVerified { get; set; }

    public bool IsPhoneVerified { get; set; }

    public byte Status { get; set; }

    public ICollection<UserRole> UserRoles { get; set; }
        = new List<UserRole>();
}