namespace BookMyShow.Application.DTOs.Venues.Venues;

public class VenueListResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string CityName { get; set; } = null!;

    public string AddressLine1 { get; set; } = null!;
}

