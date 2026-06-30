namespace BookMyShow.Application.DTOs.Bookings.SeatLocks;

public class AvailableSeatResponse
{
    public Guid SeatId { get; set; }

    public string RowName { get; set; } = null!;

    public int SeatNumber { get; set; }

    public Guid SeatCategoryId { get; set; }

    public string SeatCategoryName { get; set; } = null!;

    public decimal Price { get; set; }

    public bool IsLockedByCurrentUser { get; set; }

    public DateTime? LockedUntil { get; set; }
}

