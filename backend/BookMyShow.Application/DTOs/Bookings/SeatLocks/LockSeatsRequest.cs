namespace BookMyShow.Application.DTOs.Bookings.SeatLocks;

public class LockSeatsRequest
{
    public Guid ShowId { get; set; }

    public IReadOnlyCollection<Guid> SeatIds { get; set; } = Array.Empty<Guid>();
}

