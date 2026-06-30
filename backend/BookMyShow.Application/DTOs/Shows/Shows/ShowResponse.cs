using BookMyShow.Application.DTOs.Shows.ShowPrices;
using BookMyShow.Domain.Enums;

namespace BookMyShow.Application.DTOs.Shows.Shows;

public class ShowResponse
{
    public Guid Id { get; set; }

    public Guid? MovieId { get; set; }

    public string? MovieTitle { get; set; }

    public Guid? EventId { get; set; }

    public Guid ScreenId { get; set; }

    public string ScreenName { get; set; } = null!;

    public Guid VenueId { get; set; }

    public string VenueName { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public ShowStatus Status { get; set; }

    public IReadOnlyCollection<ShowPriceResponse> Prices { get; set; } = Array.Empty<ShowPriceResponse>();
}

