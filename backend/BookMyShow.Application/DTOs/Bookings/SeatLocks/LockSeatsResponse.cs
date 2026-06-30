namespace BookMyShow.Application.DTOs.Bookings.SeatLocks;

public class LockSeatsResponse
{
    public Guid ShowId { get; set; }

    public DateTime LockedUntil { get; set; }

    public IReadOnlyCollection<SeatLockItemResponse> Seats { get; set; } = Array.Empty<SeatLockItemResponse>();
}

