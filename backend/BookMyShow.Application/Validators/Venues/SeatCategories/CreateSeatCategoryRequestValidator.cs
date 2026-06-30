using BookMyShow.Application.DTOs.Venues.SeatCategories;
using FluentValidation;

namespace BookMyShow.Application.Validators.Venues.SeatCategories;

public class CreateSeatCategoryRequestValidator : AbstractValidator<CreateSeatCategoryRequest>
{
    public CreateSeatCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}
