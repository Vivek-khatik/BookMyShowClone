using BookMyShow.Domain.Common;

namespace BookMyShow.Domain.Entities.Movies;

public class Actor : BaseEntity
{
    public string FullName { get; set; } = null!;

    public string? ProfileImageUrl { get; set; }

    public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
}
