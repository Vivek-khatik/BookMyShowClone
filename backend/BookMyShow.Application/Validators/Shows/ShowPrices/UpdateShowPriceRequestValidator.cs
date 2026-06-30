using BookMyShow.Application.DTOs.Shows.ShowPrices;
using FluentValidation;

namespace BookMyShow.Application.Validators.Shows.ShowPrices;

public class UpdateShowPriceRequestValidator : AbstractValidator<UpdateShowPriceRequest>
{
    public UpdateShowPriceRequestValidator()
    {
        RuleFor(x => x.SeatCategoryId)
            .NotEmpty();

        RuleFor(x => x.Price)
            .GreaterThan(0);
    }
}
