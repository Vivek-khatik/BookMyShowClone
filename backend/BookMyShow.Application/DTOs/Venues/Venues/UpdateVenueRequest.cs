namespace BookMyShow.Application.DTOs.Venues.Venues;

public class UpdateVenueRequest
{
    public string Name { get; set; } = null!;

    public Guid CityId { get; set; }

    public string AddressLine1 { get; set; } = null!;

    public string? AddressLine2 { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }
}

