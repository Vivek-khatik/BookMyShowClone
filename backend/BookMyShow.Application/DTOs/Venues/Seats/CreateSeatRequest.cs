namespace BookMyShow.Application.DTOs.Venues.Seats;

public class CreateSeatRequest
{
    public Guid ScreenId { get; set; }

    public string RowName { get; set; } = null!;

    public int SeatNumber { get; set; }

    public Guid CategoryId { get; set; }
}

