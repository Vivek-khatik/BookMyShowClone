using BookMyShow.Application.DTOs.Bookings.SeatLocks;
using FluentValidation;

namespace BookMyShow.Application.Validators.Bookings.SeatLocks;

public class LockSeatsRequestValidator : AbstractValidator<LockSeatsRequest>
{
    public LockSeatsRequestValidator()
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
