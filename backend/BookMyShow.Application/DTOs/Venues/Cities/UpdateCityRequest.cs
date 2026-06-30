namespace BookMyShow.Application.DTOs.Venues.Cities;

public class UpdateCityRequest
{
    public string Name { get; set; } = null!;

    public string StateName { get; set; } = null!;

    public string CountryName { get; set; } = null!;

    public bool IsActive { get; set; }
}
