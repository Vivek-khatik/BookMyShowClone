using BookMyShow.Application.DTOs.Venues.Screens;

namespace BookMyShow.Application.Interfaces;

public interface IScreenService
{
    Task<ScreenResponse> CreateScreenAsync(CreateScreenRequest request);

    Task<ScreenResponse> UpdateScreenAsync(Guid id, UpdateScreenRequest request);

    Task DeleteScreenAsync(Guid id);

    Task<ScreenResponse?> GetScreenByIdAsync(Guid id);

    Task<IReadOnlyCollection<ScreenResponse>> GetScreensByVenueAsync(Guid venueId);
}

