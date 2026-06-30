namespace BookMyShow.Application.DTOs.Bookings.Bookings;

public class BookingSeatResponse
{
    public Guid SeatId { get; set; }

    public string RowName { get; set; } = null!;

    public int SeatNumber { get; set; }

    public Guid SeatCategoryId { get; set; }

    public string SeatCategoryName { get; set; } = null!;

    public decimal Price { get; set; }
}

