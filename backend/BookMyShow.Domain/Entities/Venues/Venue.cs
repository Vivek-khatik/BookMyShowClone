using BookMyShow.Domain.Common;

namespace BookMyShow.Domain.Entities.Venues;

public class Venue : BaseEntity
{
    public string Name { get; set; } = null!;

    public Guid CityId { get; set; }

    public City City { get; set; } = null!;

    public string AddressLine1 { get; set; } = null!;

    public string? AddressLine2 { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public ICollection<Screen> Screens { get; set; } = new List<Screen>();
}

