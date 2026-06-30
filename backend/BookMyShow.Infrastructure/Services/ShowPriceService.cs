using BookMyShow.Application.DTOs.Shows.ShowPrices;
using BookMyShow.Application.Interfaces;
using BookMyShow.Domain.Entities.Shows;
using BookMyShow.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMyShow.Infrastructure.Services;

public class ShowPriceService : IShowPriceService
{
    private readonly ApplicationDbContext _dbContext;

    public ShowPriceService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ShowPriceResponse> CreateShowPriceAsync(CreateShowPriceRequest request)
    {
        await ValidateShowPriceReferencesAsync(request.ShowId, request.SeatCategoryId, null);

        var showPrice = new ShowPrice
        {
            ShowId = request.ShowId,
            SeatCategoryId = request.SeatCategoryId,
            Price = request.Price,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.ShowPrices.Add(showPrice);
        await _dbContext.SaveChangesAsync();

        return await GetShowPriceByIdAsync(showPrice.Id)
            ?? throw new InvalidOperationException("Show price was created but could not be loaded.");
    }

    public async Task<ShowPriceResponse> UpdateShowPriceAsync(Guid id, UpdateShowPriceRequest request)
    {
        var showPrice = await _dbContext.ShowPrices
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (showPrice is null)
        {
            throw new KeyNotFoundException("Show price was not found.");
        }

        await ValidateShowPriceReferencesAsync(showPrice.ShowId, request.SeatCategoryId, id);

        showPrice.SeatCategoryId = request.SeatCategoryId;
        showPrice.Price = request.Price;
        showPrice.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return await GetShowPriceByIdAsync(showPrice.Id)
            ?? throw new InvalidOperationException("Show price was updated but could not be loaded.");
    }

    public async Task DeleteShowPriceAsync(Guid id)
    {
        var showPrice = await _dbContext.ShowPrices
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (showPrice is null)
        {
            throw new KeyNotFoundException("Show price was not found.");
        }

        showPrice.IsDeleted = true;
        showPrice.DeletedAt = DateTime.UtcNow;
        showPrice.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<ShowPriceResponse>> GetShowPricesAsync(Guid showId)
    {
        var showExists = await _dbContext.Shows
            .AnyAsync(x => x.Id == showId && !x.IsDeleted);

        if (!showExists)
        {
            throw new KeyNotFoundException("Show was not found.");
        }

        return await _dbContext.ShowPrices
            .AsNoTracking()
            .Where(x => x.ShowId == showId && !x.IsDeleted)
            .OrderBy(x => x.SeatCategoryId)
            .Select(x => new ShowPriceResponse
            {
                Id = x.Id,
                ShowId = x.ShowId,
                SeatCategoryId = x.SeatCategoryId,
                SeatCategoryName = x.SeatCategory.Name,
                Price = x.Price
            })
            .ToArrayAsync();
    }

    private async Task<ShowPriceResponse?> GetShowPriceByIdAsync(Guid id)
    {
        return await _dbContext.ShowPrices
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new ShowPriceResponse
            {
                Id = x.Id,
                ShowId = x.ShowId,
                SeatCategoryId = x.SeatCategoryId,
                SeatCategoryName = x.SeatCategory.Name,
                Price = x.Price
            })
            .SingleOrDefaultAsync();
    }

    private async Task ValidateShowPriceReferencesAsync(Guid showId, Guid seatCategoryId, Guid? existingShowPriceId)
    {
        var showExists = await _dbContext.Shows
            .AnyAsync(x => x.Id == showId && !x.IsDeleted);

        if (!showExists)
        {
            throw new InvalidOperationException("Show was not found.");
        }

        var categoryExists = await _dbContext.SeatCategories
            .AnyAsync(x => x.Id == seatCategoryId && !x.IsDeleted);

        if (!categoryExists)
        {
            throw new InvalidOperationException("Seat category was not found.");
        }

        var duplicateExists = await _dbContext.ShowPrices
            .AnyAsync(x =>
                !x.IsDeleted &&
                x.ShowId == showId &&
                x.SeatCategoryId == seatCategoryId &&
                x.Id != existingShowPriceId);

        if (duplicateExists)
        {
            throw new InvalidOperationException("Price for this seat category already exists for the show.");
        }
    }
}

