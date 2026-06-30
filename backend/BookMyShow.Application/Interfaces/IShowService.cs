using BookMyShow.Application.DTOs.Shows.Shows;

namespace BookMyShow.Application.Interfaces;

public interface IShowService
{
    Task<ShowResponse> CreateShowAsync(CreateShowRequest request);

    Task<ShowResponse> UpdateShowAsync(Guid id, UpdateShowRequest request);

    Task DeleteShowAsync(Guid id);

    Task<ShowResponse?> GetShowByIdAsync(Guid id);

    Task<IReadOnlyCollection<ShowListResponse>> GetShowsAsync();
}

