using BookMyShow.Application.DTOs.Venues.SeatCategories;

namespace BookMyShow.Application.Interfaces;

public interface ISeatCategoryService
{
    Task<SeatCategoryResponse> CreateSeatCategoryAsync(CreateSeatCategoryRequest request);

    Task<SeatCategoryResponse> UpdateSeatCategoryAsync(Guid id, UpdateSeatCategoryRequest request);

    Task DeleteSeatCategoryAsync(Guid id);

    Task<SeatCategoryResponse?> GetSeatCategoryByIdAsync(Guid id);

    Task<IReadOnlyCollection<SeatCategoryResponse>> GetSeatCategoriesAsync();
}

