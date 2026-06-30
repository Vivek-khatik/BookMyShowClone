using BookMyShow.Application.DTOs.Venues.Screens;
using FluentValidation;

namespace BookMyShow.Application.Validators.Venues.Screens;

public class UpdateScreenRequestValidator : AbstractValidator<UpdateScreenRequest>
{
    public UpdateScreenRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Capacity)
            .GreaterThan(0);
    }
}
