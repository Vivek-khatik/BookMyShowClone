using BookMyShow.Application.DTOs.Venues.Venues;
using BookMyShow.Application.Interfaces;
using BookMyShow.Domain.Entities.Venues;
using BookMyShow.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMyShow.Infrastructure.Services;

public class VenueService : IVenueService
{
    private readonly ApplicationDbContext _dbContext;

    public VenueService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<VenueResponse> CreateVenueAsync(CreateVenueRequest request)
    {
        await ValidateCityReferenceAsync(request.CityId);

        var venue = new Venue
        {
            Name = request.Name.Trim(),
            CityId = request.CityId,
            AddressLine1 = request.AddressLine1.Trim(),
            AddressLine2 = NormalizeOptionalString(request.AddressLine2),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Venues.Add(venue);
        await _dbContext.SaveChangesAsync();

        return await GetVenueByIdAsync(venue.Id)
            ?? throw new InvalidOperationException("Venue was created but could not be loaded.");
    }

    public async Task<VenueResponse> UpdateVenueAsync(Guid id, UpdateVenueRequest request)
    {
        var venue = await _dbContext.Venues
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (venue is null)
        {
            throw new KeyNotFoundException("Venue was not found.");
        }

        await ValidateCityReferenceAsync(request.CityId);

        venue.Name = request.Name.Trim();
        venue.CityId = request.CityId;
        venue.AddressLine1 = request.AddressLine1.Trim();
        venue.AddressLine2 = NormalizeOptionalString(request.AddressLine2);
        venue.Latitude = request.Latitude;
        venue.Longitude = request.Longitude;
        venue.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return await GetVenueByIdAsync(venue.Id)
            ?? throw new InvalidOperationException("Venue was updated but could not be loaded.");
    }

    public async Task DeleteVenueAsync(Guid id)
    {
        var venue = await _dbContext.Venues
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (venue is null)
        {
            throw new KeyNotFoundException("Venue was not found.");
        }

        venue.IsDeleted = true;
        venue.DeletedAt = DateTime.UtcNow;
        venue.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<VenueResponse?> GetVenueByIdAsync(Guid id)
    {
        return await _dbContext.Venues
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new VenueResponse
            {
                Id = x.Id,
                Name = x.Name,
                CityId = x.CityId,
                CityName = x.City.Name,
                AddressLine1 = x.AddressLine1,
                AddressLine2 = x.AddressLine2,
                Latitude = x.Latitude,
                Longitude = x.Longitude
            })
            .SingleOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<VenueListResponse>> GetVenuesAsync()
    {
        return await _dbContext.Venues
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new VenueListResponse
            {
                Id = x.Id,
                Name = x.Name,
                CityName = x.City.Name,
                AddressLine1 = x.AddressLine1
            })
            .ToArrayAsync();
    }

    public async Task<IReadOnlyCollection<VenueListResponse>> GetVenuesByCityAsync(Guid cityId)
    {
        return await _dbContext.Venues
            .AsNoTracking()
            .Where(x => x.CityId == cityId && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new VenueListResponse
            {
                Id = x.Id,
                Name = x.Name,
                CityName = x.City.Name,
                AddressLine1 = x.AddressLine1
            })
            .ToArrayAsync();
    }

    private async Task ValidateCityReferenceAsync(Guid cityId)
    {
        var cityExists = await _dbContext.Cities
            .AnyAsync(x => x.Id == cityId && !x.IsDeleted);

        if (!cityExists)
        {
            throw new InvalidOperationException("City was not found.");
        }
    }

    private static string? NormalizeOptionalString(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

