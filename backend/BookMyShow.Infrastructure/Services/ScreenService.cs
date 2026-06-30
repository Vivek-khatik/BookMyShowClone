using BookMyShow.Application.DTOs.Venues.Screens;
using BookMyShow.Application.Interfaces;
using BookMyShow.Domain.Entities.Venues;
using BookMyShow.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMyShow.Infrastructure.Services;

public class ScreenService : IScreenService
{
    private readonly ApplicationDbContext _dbContext;

    public ScreenService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ScreenResponse> CreateScreenAsync(CreateScreenRequest request)
    {
        await ValidateVenueReferenceAsync(request.VenueId);

        var screen = new Screen
        {
            VenueId = request.VenueId,
            Name = request.Name.Trim(),
            Capacity = request.Capacity,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Screens.Add(screen);
        await _dbContext.SaveChangesAsync();

        return await GetScreenByIdAsync(screen.Id)
            ?? throw new InvalidOperationException("Screen was created but could not be loaded.");
    }

    public async Task<ScreenResponse> UpdateScreenAsync(Guid id, UpdateScreenRequest request)
    {
        var screen = await _dbContext.Screens
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (screen is null)
        {
            throw new KeyNotFoundException("Screen was not found.");
        }

        screen.Name = request.Name.Trim();
        screen.Capacity = request.Capacity;
        screen.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return await GetScreenByIdAsync(screen.Id)
            ?? throw new InvalidOperationException("Screen was updated but could not be loaded.");
    }

    public async Task DeleteScreenAsync(Guid id)
    {
        var screen = await _dbContext.Screens
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (screen is null)
        {
            throw new KeyNotFoundException("Screen was not found.");
        }

        screen.IsDeleted = true;
        screen.DeletedAt = DateTime.UtcNow;
        screen.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<ScreenResponse?> GetScreenByIdAsync(Guid id)
    {
        return await _dbContext.Screens
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new ScreenResponse
            {
                Id = x.Id,
                VenueId = x.VenueId,
                VenueName = x.Venue.Name,
                Name = x.Name,
                Capacity = x.Capacity
            })
            .SingleOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<ScreenResponse>> GetScreensByVenueAsync(Guid venueId)
    {
        return await _dbContext.Screens
            .AsNoTracking()
            .Where(x => x.VenueId == venueId && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new ScreenResponse
            {
                Id = x.Id,
                VenueId = x.VenueId,
                VenueName = x.Venue.Name,
                Name = x.Name,
                Capacity = x.Capacity
            })
            .ToArrayAsync();
    }

    private async Task ValidateVenueReferenceAsync(Guid venueId)
    {
        var venueExists = await _dbContext.Venues
            .AnyAsync(x => x.Id == venueId && !x.IsDeleted);

        if (!venueExists)
        {
            throw new InvalidOperationException("Venue was not found.");
        }
    }
}

