using BookMyShow.Application.DTOs.Shows.ShowPrices;
using BookMyShow.Application.DTOs.Shows.Shows;
using BookMyShow.Application.Interfaces;
using BookMyShow.Domain.Entities.Shows;
using BookMyShow.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMyShow.Infrastructure.Services;

public class ShowService : IShowService
{
    private readonly ApplicationDbContext _dbContext;

    public ShowService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ShowResponse> CreateShowAsync(CreateShowRequest request)
    {
        await ValidateShowReferencesAsync(
            request.MovieId,
            request.EventId,
            request.ScreenId,
            request.StartTime,
            request.EndTime,
            null);

        var show = new Show
        {
            MovieId = request.MovieId,
            EventId = request.EventId,
            ScreenId = request.ScreenId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Shows.Add(show);
        await _dbContext.SaveChangesAsync();

        return await GetShowByIdAsync(show.Id)
            ?? throw new InvalidOperationException("Show was created but could not be loaded.");
    }

    public async Task<ShowResponse> UpdateShowAsync(Guid id, UpdateShowRequest request)
    {
        var show = await _dbContext.Shows
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (show is null)
        {
            throw new KeyNotFoundException("Show was not found.");
        }

        await ValidateShowReferencesAsync(
            request.MovieId,
            request.EventId,
            request.ScreenId,
            request.StartTime,
            request.EndTime,
            id);

        show.MovieId = request.MovieId;
        show.EventId = request.EventId;
        show.ScreenId = request.ScreenId;
        show.StartTime = request.StartTime;
        show.EndTime = request.EndTime;
        show.Status = request.Status;
        show.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return await GetShowByIdAsync(show.Id)
            ?? throw new InvalidOperationException("Show was updated but could not be loaded.");
    }

    public async Task DeleteShowAsync(Guid id)
    {
        var show = await _dbContext.Shows
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (show is null)
        {
            throw new KeyNotFoundException("Show was not found.");
        }

        show.IsDeleted = true;
        show.DeletedAt = DateTime.UtcNow;
        show.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<ShowResponse?> GetShowByIdAsync(Guid id)
    {
        return await _dbContext.Shows
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new ShowResponse
            {
                Id = x.Id,
                MovieId = x.MovieId,
                MovieTitle = x.Movie != null ? x.Movie.Title : null,
                EventId = x.EventId,
                ScreenId = x.ScreenId,
                ScreenName = x.Screen.Name,
                VenueId = x.Screen.VenueId,
                VenueName = x.Screen.Venue.Name,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Status = x.Status,
                Prices = x.ShowPrices
                    .Where(showPrice => !showPrice.IsDeleted)
                    .OrderBy(showPrice => showPrice.SeatCategoryId)
                    .Select(showPrice => new ShowPriceResponse
                    {
                        Id = showPrice.Id,
                        ShowId = showPrice.ShowId,
                        SeatCategoryId = showPrice.SeatCategoryId,
                        SeatCategoryName = showPrice.SeatCategory.Name,
                        Price = showPrice.Price
                    })
                    .ToArray()
            })
            .SingleOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<ShowListResponse>> GetShowsAsync()
    {
        return await _dbContext.Shows
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.StartTime)
            .ThenBy(x => x.Screen.Name)
            .Select(x => new ShowListResponse
            {
                Id = x.Id,
                MovieId = x.MovieId,
                MovieTitle = x.Movie != null ? x.Movie.Title : null,
                ScreenId = x.ScreenId,
                ScreenName = x.Screen.Name,
                VenueId = x.Screen.VenueId,
                VenueName = x.Screen.Venue.Name,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Status = x.Status
            })
            .ToArrayAsync();
    }

    private async Task ValidateShowReferencesAsync(
        Guid? movieId,
        Guid? eventId,
        Guid screenId,
        DateTime startTime,
        DateTime endTime,
        Guid? existingShowId)
    {
        if (movieId is null)
        {
            throw new InvalidOperationException("MovieId is required for shows.");
        }

        if (eventId is not null)
        {
            throw new InvalidOperationException("Event support is not implemented yet.");
        }

        var movieExists = await _dbContext.Movies
            .AnyAsync(x => x.Id == movieId.Value && !x.IsDeleted);

        if (!movieExists)
        {
            throw new InvalidOperationException("Movie was not found.");
        }

        var screenExists = await _dbContext.Screens
            .AnyAsync(x => x.Id == screenId && !x.IsDeleted);

        if (!screenExists)
        {
            throw new InvalidOperationException("Screen was not found.");
        }

        var hasOverlap = await _dbContext.Shows
            .AnyAsync(x =>
                !x.IsDeleted &&
                x.ScreenId == screenId &&
                x.Id != existingShowId &&
                startTime < x.EndTime &&
                endTime > x.StartTime);

        if (hasOverlap)
        {
            throw new InvalidOperationException("Show timing overlaps with an existing show on the same screen.");
        }
    }
}

