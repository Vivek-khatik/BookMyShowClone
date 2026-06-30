namespace BookMyShow.Application.DTOs.Venues.Seats;

public class SeatResponse
{
    public Guid Id { get; set; }

    public Guid ScreenId { get; set; }

    public string ScreenName { get; set; } = null!;

    public string RowName { get; set; } = null!;

    public int SeatNumber { get; set; }

    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;
}

