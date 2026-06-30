using BookMyShow.Domain.Common;

namespace BookMyShow.Domain.Entities.Movies;

public class Genre : BaseEntity
{
    public string Name { get; set; } = null!;

    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
