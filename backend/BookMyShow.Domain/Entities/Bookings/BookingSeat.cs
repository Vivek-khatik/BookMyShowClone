using BookMyShow.Domain.Entities.Venues;

namespace BookMyShow.Domain.Entities.Bookings;

public class BookingSeat
{
    public Guid BookingId { get; set; }

    public Booking Booking { get; set; } = null!;

    public Guid SeatId { get; set; }

    public Seat Seat { get; set; } = null!;
}

