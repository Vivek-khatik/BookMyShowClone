using BookMyShow.Application.DTOs.Venues.Venues;
using FluentValidation;

namespace BookMyShow.Application.Validators.Venues.Venues;

public class CreateVenueRequestValidator : AbstractValidator<CreateVenueRequest>
{
    public CreateVenueRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.CityId)
            .NotEmpty();

        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.AddressLine2)
            .MaximumLength(500);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180);
    }
}
