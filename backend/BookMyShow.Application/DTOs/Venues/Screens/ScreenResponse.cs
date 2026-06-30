namespace BookMyShow.Application.DTOs.Venues.Screens;

public class ScreenResponse
{
    public Guid Id { get; set; }

    public Guid VenueId { get; set; }

    public string VenueName { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Capacity { get; set; }
}

