using BookMyShow.Application.DTOs.Movies.Languages;
using FluentValidation;

namespace BookMyShow.Application.Validators.Movies.Languages;

public class CreateLanguageRequestValidator : AbstractValidator<CreateLanguageRequest>
{
    public CreateLanguageRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
