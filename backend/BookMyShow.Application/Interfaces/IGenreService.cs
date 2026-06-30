using BookMyShow.Application.DTOs.Common;
using BookMyShow.Application.DTOs.Movies.Genres;
using System.Threading.Tasks;

namespace BookMyShow.Application.Interfaces;

public interface IGenreService
{
    Task<GenreResponse> CreateGenreAsync(CreateGenreRequest request);

    Task<GenreResponse> UpdateGenreAsync(Guid id, UpdateGenreRequest request);

    Task DeleteGenreAsync(Guid id);

    Task<GenreResponse?> GetGenreByIdAsync(Guid id);

    Task<PaginatedResponse<GenreListResponse>> GetGenresAsync(
        string? search,
        int pageNumber,
        int pageSize,
        string? sortBy,
        string? sortDirection);
}

