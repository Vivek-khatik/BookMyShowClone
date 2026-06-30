using BookMyShow.Application.DTOs.Bookings.Bookings;
using FluentValidation;

namespace BookMyShow.Application.Validators.Bookings.Bookings;

public class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
{
    public CreateBookingRequestValidator()
    {
        RuleFor(x => x.ShowId)
            .NotEmpty();

        RuleFor(x => x.SeatIds)
            .NotEmpty();

        RuleForEach(x => x.SeatIds)
            .NotEmpty();

        RuleFor(x => x.SeatIds)
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("SeatIds cannot contain duplicate values.");
    }
}
