namespace BookMyShow.Application.DTOs.Venues.Seats;

public class UpdateSeatRequest
{
    public string RowName { get; set; } = null!;

    public int SeatNumber { get; set; }

    public Guid CategoryId { get; set; }
}

