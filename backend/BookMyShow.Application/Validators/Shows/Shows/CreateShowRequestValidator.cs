using BookMyShow.Application.DTOs.Shows.Shows;
using FluentValidation;

namespace BookMyShow.Application.Validators.Shows.Shows;

public class CreateShowRequestValidator : AbstractValidator<CreateShowRequest>
{
    public CreateShowRequestValidator()
    {
        RuleFor(x => x.ScreenId)
            .NotEmpty();

        RuleFor(x => x.StartTime)
            .NotEmpty();

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("EndTime must be greater than StartTime.");

        RuleFor(x => x.Status)
            .IsInEnum();
    }
}
