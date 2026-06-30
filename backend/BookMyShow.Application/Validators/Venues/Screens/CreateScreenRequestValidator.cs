using BookMyShow.Application.DTOs.Venues.Screens;
using FluentValidation;

namespace BookMyShow.Application.Validators.Venues.Screens;

public class CreateScreenRequestValidator : AbstractValidator<CreateScreenRequest>
{
    public CreateScreenRequestValidator()
    {
        RuleFor(x => x.VenueId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Capacity)
            .GreaterThan(0);
    }
}
