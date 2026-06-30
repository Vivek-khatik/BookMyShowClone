namespace BookMyShow.Application.DTOs.Venues.Screens;

public class CreateScreenRequest
{
    public Guid VenueId { get; set; }

    public string Name { get; set; } = null!;

    public int Capacity { get; set; }
}

