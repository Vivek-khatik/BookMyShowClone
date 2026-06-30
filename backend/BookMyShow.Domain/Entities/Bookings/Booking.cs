using BookMyShow.Domain.Common;
using BookMyShow.Domain.Entities.Identity;
using BookMyShow.Domain.Entities.Shows;
using BookMyShow.Domain.Enums;

namespace BookMyShow.Domain.Entities.Bookings;

public class Booking : BaseEntity
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Guid ShowId { get; set; }

    public Show Show { get; set; } = null!;

    public string BookingReference { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public BookingStatus Status { get; set; }

    public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
}

