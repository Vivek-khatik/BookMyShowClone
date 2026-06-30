using BookMyShow.Domain.Enums;

namespace BookMyShow.Application.DTOs.Movies;

public class UpdateMovieRequest
{
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int DurationMinutes { get; set; }

    public DateOnly ReleaseDate { get; set; }

    public string? PosterUrl { get; set; }

    public string? BannerUrl { get; set; }

    public string? TrailerUrl { get; set; }

    public string CensorRating { get; set; } = null!;

    public Guid GenreId { get; set; }

    public Guid LanguageId { get; set; }

    public MovieStatus Status { get; set; }

    public IReadOnlyCollection<Guid> ActorIds { get; set; } = Array.Empty<Guid>();
}

