using BookMyShow.Domain.Enums;

namespace BookMyShow.Application.DTOs.Bookings.Bookings;

public class BookingSummaryResponse
{
    public Guid Id { get; set; }

    public string BookingReference { get; set; } = null!;

    public Guid ShowId { get; set; }

    public string? MovieTitle { get; set; }

    public string ScreenName { get; set; } = null!;

    public string VenueName { get; set; } = null!;

    public DateTime ShowStartTime { get; set; }

    public decimal TotalAmount { get; set; }

    public BookingStatus Status { get; set; }
}

