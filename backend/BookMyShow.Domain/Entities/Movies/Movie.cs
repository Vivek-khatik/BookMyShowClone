using BookMyShow.Domain.Common;
using BookMyShow.Domain.Enums;

namespace BookMyShow.Domain.Entities.Movies;

public class Movie : BaseEntity
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

    public Genre Genre { get; set; } = null!;

    public Guid LanguageId { get; set; }

    public Language Language { get; set; } = null!;

    public MovieStatus Status { get; set; }

    public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
}

