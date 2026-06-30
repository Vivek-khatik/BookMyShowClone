using BookMyShow.Domain.Common;

namespace BookMyShow.Domain.Entities.Identity;

public class Role : BaseEntity
{
    public string Name { get; set; } = null!;

    public ICollection<UserRole> UserRoles { get; set; }
        = new List<UserRole>();
}