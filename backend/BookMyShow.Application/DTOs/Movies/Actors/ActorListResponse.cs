namespace BookMyShow.Application.DTOs.Movies.Actors;

public class ActorListResponse
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;

    public string? ProfileImageUrl { get; set; }
}

