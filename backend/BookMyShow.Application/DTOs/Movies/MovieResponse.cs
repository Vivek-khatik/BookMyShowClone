using BookMyShow.Domain.Enums;
using BookMyShow.Application.DTOs.Movies.Genres;
using BookMyShow.Application.DTOs.Movies.Languages;
using BookMyShow.Application.DTOs.Movies.Actors;

namespace BookMyShow.Application.DTOs.Movies;

public class MovieResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int DurationMinutes { get; set; }

    public DateOnly ReleaseDate { get; set; }

    public string? PosterUrl { get; set; }

    public string? BannerUrl { get; set; }

    public string? TrailerUrl { get; set; }

    public string CensorRating { get; set; } = null!;

    public GenreResponse Genre { get; set; } = null!;

    public LanguageResponse Language { get; set; } = null!;

    public MovieStatus Status { get; set; }

    public IReadOnlyCollection<ActorResponse> Actors { get; set; } = Array.Empty<ActorResponse>();
}

