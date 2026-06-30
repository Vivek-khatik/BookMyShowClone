using BookMyShow.Domain.Common;

namespace BookMyShow.Domain.Entities.Venues;

public class Seat : BaseEntity
{
    public Guid ScreenId { get; set; }

    public Screen Screen { get; set; } = null!;

    public string RowName { get; set; } = null!;

    public int SeatNumber { get; set; }

    public Guid CategoryId { get; set; }

    public SeatCategory Category { get; set; } = null!;
}

