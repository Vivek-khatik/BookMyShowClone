namespace BookMyShow.Application.DTOs.Movies.Actors;

public class ActorResponse
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;

    public string? ProfileImageUrl { get; set; }
}

