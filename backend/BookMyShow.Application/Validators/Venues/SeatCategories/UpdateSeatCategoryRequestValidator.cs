using BookMyShow.Application.DTOs.Venues.SeatCategories;
using FluentValidation;

namespace BookMyShow.Application.Validators.Venues.SeatCategories;

public class UpdateSeatCategoryRequestValidator : AbstractValidator<UpdateSeatCategoryRequest>
{
    public UpdateSeatCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}
