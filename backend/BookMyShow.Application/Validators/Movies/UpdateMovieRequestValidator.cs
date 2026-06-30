using BookMyShow.Application.DTOs.Movies;
using FluentValidation;

namespace BookMyShow.Application.Validators.Movies;

public class UpdateMovieRequestValidator : AbstractValidator<UpdateMovieRequest>
{
    public UpdateMovieRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0)
            .LessThanOrEqualTo(600);

        RuleFor(x => x.ReleaseDate)
            .NotEmpty();

        RuleFor(x => x.PosterUrl)
            .MaximumLength(1000);

        RuleFor(x => x.BannerUrl)
            .MaximumLength(1000);

        RuleFor(x => x.TrailerUrl)
            .MaximumLength(1000);

        RuleFor(x => x.CensorRating)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.GenreId)
            .NotEmpty();

        RuleFor(x => x.LanguageId)
            .NotEmpty();

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.ActorIds)
            .Must(x => x.All(id => id != Guid.Empty))
            .WithMessage("ActorIds cannot contain empty values.")
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("ActorIds cannot contain duplicate values.");
    }
}
