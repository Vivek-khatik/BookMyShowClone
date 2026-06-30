using BookMyShow.Domain.Common;
using BookMyShow.Domain.Entities.Venues;

namespace BookMyShow.Domain.Entities.Shows;

public class ShowPrice : BaseEntity
{
    public Guid ShowId { get; set; }

    public Show Show { get; set; } = null!;

    public Guid SeatCategoryId { get; set; }

    public SeatCategory SeatCategory { get; set; } = null!;

    public decimal Price { get; set; }
}

