using BookMyShow.Application.DTOs.Venues.SeatCategories;
using BookMyShow.Application.Interfaces;
using BookMyShow.Domain.Entities.Venues;
using BookMyShow.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMyShow.Infrastructure.Services;

public class SeatCategoryService : ISeatCategoryService
{
    private readonly ApplicationDbContext _dbContext;

    public SeatCategoryService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SeatCategoryResponse> CreateSeatCategoryAsync(CreateSeatCategoryRequest request)
    {
        var category = new SeatCategory
        {
            Name = request.Name.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.SeatCategories.Add(category);
        await _dbContext.SaveChangesAsync();

        return await GetSeatCategoryByIdAsync(category.Id)
            ?? throw new InvalidOperationException("Seat category was created but could not be loaded.");
    }

    public async Task<SeatCategoryResponse> UpdateSeatCategoryAsync(Guid id, UpdateSeatCategoryRequest request)
    {
        var category = await _dbContext.SeatCategories
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (category is null)
        {
            throw new KeyNotFoundException("Seat category was not found.");
        }

        category.Name = request.Name.Trim();
        category.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return await GetSeatCategoryByIdAsync(category.Id)
            ?? throw new InvalidOperationException("Seat category was updated but could not be loaded.");
    }

    public async Task DeleteSeatCategoryAsync(Guid id)
    {
        var category = await _dbContext.SeatCategories
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (category is null)
        {
            throw new KeyNotFoundException("Seat category was not found.");
        }

        category.IsDeleted = true;
        category.DeletedAt = DateTime.UtcNow;
        category.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<SeatCategoryResponse?> GetSeatCategoryByIdAsync(Guid id)
    {
        return await _dbContext.SeatCategories
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new SeatCategoryResponse
            {
                Id = x.Id,
                Name = x.Name
            })
            .SingleOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<SeatCategoryResponse>> GetSeatCategoriesAsync()
    {
        return await _dbContext.SeatCategories
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Id)
            .Select(x => new SeatCategoryResponse
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToArrayAsync();
    }
}

