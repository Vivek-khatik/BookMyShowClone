namespace BookMyShow.Application.DTOs.Movies.Actors;

public class UpdateActorRequest
{
    public string FullName { get; set; } = null!;

    public string? ProfileImageUrl { get; set; }
}
