using BookMyShow.Application.DTOs.Movies.Genres;
using FluentValidation;

namespace BookMyShow.Application.Validators.Movies.Genres;

public class UpdateGenreRequestValidator : AbstractValidator<UpdateGenreRequest>
{
    public UpdateGenreRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
