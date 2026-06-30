using BookMyShow.Domain.Common;
using BookMyShow.Domain.Entities.Movies;
using BookMyShow.Domain.Entities.Venues;
using BookMyShow.Domain.Enums;

namespace BookMyShow.Domain.Entities.Shows;

public class Show : BaseEntity
{
    public Guid? MovieId { get; set; }

    public Movie? Movie { get; set; }

    public Guid? EventId { get; set; }

    public Guid ScreenId { get; set; }

    public Screen Screen { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public ShowStatus Status { get; set; }

    public ICollection<ShowPrice> ShowPrices { get; set; } = new List<ShowPrice>();
}

