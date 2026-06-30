using BookMyShow.Domain.Common;
using BookMyShow.Domain.Entities.Identity;
using BookMyShow.Domain.Entities.Shows;
using BookMyShow.Domain.Entities.Venues;

namespace BookMyShow.Domain.Entities.Bookings;

public class SeatLock : BaseEntity
{
    public Guid ShowId { get; set; }

    public Show Show { get; set; } = null!;

    public Guid SeatId { get; set; }

    public Seat Seat { get; set; } = null!;

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public DateTime LockedUntil { get; set; }
}

