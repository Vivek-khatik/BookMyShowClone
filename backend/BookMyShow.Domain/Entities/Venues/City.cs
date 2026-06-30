using BookMyShow.Domain.Common;

namespace BookMyShow.Domain.Entities.Venues;

public class City : BaseEntity
{
    public string Name { get; set; } = null!;

    public string StateName { get; set; } = null!;

    public string CountryName { get; set; } = null!;

    public bool IsActive { get; set; }

    public ICollection<Venue> Venues { get; set; } = new List<Venue>();
}
