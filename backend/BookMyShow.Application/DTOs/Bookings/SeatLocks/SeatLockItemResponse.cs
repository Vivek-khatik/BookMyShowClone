namespace BookMyShow.Application.DTOs.Bookings.SeatLocks;

public class SeatLockItemResponse
{
    public Guid SeatLockId { get; set; }

    public Guid SeatId { get; set; }

    public string RowName { get; set; } = null!;

    public int SeatNumber { get; set; }

    public string SeatCategoryName { get; set; } = null!;

    public DateTime LockedUntil { get; set; }
}

