namespace BookMyShow.Application.DTOs.Venues.Cities;

public class CityResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string StateName { get; set; } = null!;

    public string CountryName { get; set; } = null!;

    public bool IsActive { get; set; }
}

