using BookMyShow.Domain.Enums;

namespace BookMyShow.Application.DTOs.Bookings.Bookings;

public class BookingResponse
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid ShowId { get; set; }

    public string BookingReference { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public BookingStatus Status { get; set; }

    public string? MovieTitle { get; set; }

    public string ScreenName { get; set; } = null!;

    public string VenueName { get; set; } = null!;

    public DateTime ShowStartTime { get; set; }

    public DateTime ShowEndTime { get; set; }

    public IReadOnlyCollection<BookingSeatResponse> Seats { get; set; } = Array.Empty<BookingSeatResponse>();
}

