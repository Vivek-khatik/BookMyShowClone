using BookMyShow.Application.DTOs.Venues.Seats;
using FluentValidation;

namespace BookMyShow.Application.Validators.Venues.Seats;

public class UpdateSeatRequestValidator : AbstractValidator<UpdateSeatRequest>
{
    public UpdateSeatRequestValidator()
    {
        RuleFor(x => x.RowName)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.SeatNumber)
            .GreaterThan(0);

        RuleFor(x => x.CategoryId)
            .NotEmpty();
    }
}
