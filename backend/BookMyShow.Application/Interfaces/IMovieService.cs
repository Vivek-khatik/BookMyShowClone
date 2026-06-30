using BookMyShow.Application.DTOs.Movies;

namespace BookMyShow.Application.Interfaces;

public interface IMovieService
{
    Task<MovieResponse> CreateMovieAsync(CreateMovieRequest request);

    Task<MovieResponse> UpdateMovieAsync(Guid id, UpdateMovieRequest request);

    Task DeleteMovieAsync(Guid id);

    Task<MovieResponse?> GetMovieByIdAsync(Guid id);

    Task<IReadOnlyCollection<MovieListResponse>> GetMoviesAsync();
}

