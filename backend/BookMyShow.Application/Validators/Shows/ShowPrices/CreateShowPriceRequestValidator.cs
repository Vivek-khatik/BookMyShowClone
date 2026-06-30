using BookMyShow.Application.DTOs.Shows.ShowPrices;
using FluentValidation;

namespace BookMyShow.Application.Validators.Shows.ShowPrices;

public class CreateShowPriceRequestValidator : AbstractValidator<CreateShowPriceRequest>
{
    public CreateShowPriceRequestValidator()
    {
        RuleFor(x => x.ShowId)
            .NotEmpty();

        RuleFor(x => x.SeatCategoryId)
            .NotEmpty();

        RuleFor(x => x.Price)
            .GreaterThan(0);
    }
}
