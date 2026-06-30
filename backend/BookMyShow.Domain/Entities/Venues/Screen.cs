using BookMyShow.Domain.Common;

namespace BookMyShow.Domain.Entities.Venues;

public class Screen : BaseEntity
{
    public Guid VenueId { get; set; }

    public Venue Venue { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Capacity { get; set; }

    public ICollection<Seat> Seats { get; set; } = new List<Seat>();
}

