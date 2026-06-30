namespace BookMyShow.Application.DTOs.Bookings.Bookings;

public class CreateBookingRequest
{
    public Guid ShowId { get; set; }

    public IReadOnlyCollection<Guid> SeatIds { get; set; } = Array.Empty<Guid>();
}

