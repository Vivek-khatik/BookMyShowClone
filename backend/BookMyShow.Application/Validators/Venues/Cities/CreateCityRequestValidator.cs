using BookMyShow.Application.DTOs.Venues.Cities;
using FluentValidation;

namespace BookMyShow.Application.Validators.Venues.Cities;

public class CreateCityRequestValidator : AbstractValidator<CreateCityRequest>
{
    public CreateCityRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.StateName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.CountryName)
            .NotEmpty()
            .MaximumLength(100);
    }
}
