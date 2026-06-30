namespace BookMyShow.Application.DTOs.Movies.Actors;

public class CreateActorRequest
{
    public string FullName { get; set; } = null!;

    public string? ProfileImageUrl { get; set; }
}
