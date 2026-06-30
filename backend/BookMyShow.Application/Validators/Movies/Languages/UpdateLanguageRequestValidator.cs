using BookMyShow.Application.DTOs.Movies.Languages;
using FluentValidation;

namespace BookMyShow.Application.Validators.Movies.Languages;

public class UpdateLanguageRequestValidator : AbstractValidator<UpdateLanguageRequest>
{
    public UpdateLanguageRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
