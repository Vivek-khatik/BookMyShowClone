using BookMyShow.Application.DTOs.Shows.ShowPrices;

namespace BookMyShow.Application.Interfaces;

public interface IShowPriceService
{
    Task<ShowPriceResponse> CreateShowPriceAsync(CreateShowPriceRequest request);

    Task<ShowPriceResponse> UpdateShowPriceAsync(Guid id, UpdateShowPriceRequest request);

    Task DeleteShowPriceAsync(Guid id);

    Task<IReadOnlyCollection<ShowPriceResponse>> GetShowPricesAsync(Guid showId);
}

