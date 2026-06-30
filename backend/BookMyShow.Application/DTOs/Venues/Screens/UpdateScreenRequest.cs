namespace BookMyShow.Application.DTOs.Venues.Screens;

public class UpdateScreenRequest
{
    public string Name { get; set; } = null!;

    public int Capacity { get; set; }
}
