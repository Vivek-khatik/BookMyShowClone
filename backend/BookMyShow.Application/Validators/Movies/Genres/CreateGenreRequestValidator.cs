using BookMyShow.Application.DTOs.Movies.Genres;
using FluentValidation;

namespace BookMyShow.Application.Validators.Movies.Genres;

public class CreateGenreRequestValidator : AbstractValidator<CreateGenreRequest>
{
    public CreateGenreRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
