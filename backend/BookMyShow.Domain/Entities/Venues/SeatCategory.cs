using BookMyShow.Domain.Common;

namespace BookMyShow.Domain.Entities.Venues;

public class SeatCategory : BaseEntity
{
    public string Name { get; set; } = null!;

    public ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
