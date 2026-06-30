using BookMyShow.Application.DTOs.Venues.Cities;
using BookMyShow.Application.Interfaces;
using BookMyShow.Domain.Entities.Venues;
using BookMyShow.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMyShow.Infrastructure.Services;

public class CityService : ICityService
{
    private readonly ApplicationDbContext _dbContext;

    public CityService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CityResponse> CreateCityAsync(CreateCityRequest request)
    {
        var city = new City
        {
            Name = request.Name.Trim(),
            StateName = request.StateName.Trim(),
            CountryName = request.CountryName.Trim(),
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Cities.Add(city);
        await _dbContext.SaveChangesAsync();

        return await GetCityByIdAsync(city.Id)
            ?? throw new InvalidOperationException("City was created but could not be loaded.");
    }

    public async Task<CityResponse> UpdateCityAsync(Guid id, UpdateCityRequest request)
    {
        var city = await _dbContext.Cities
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (city is null)
        {
            throw new KeyNotFoundException("City was not found.");
        }

        city.Name = request.Name.Trim();
        city.StateName = request.StateName.Trim();
        city.CountryName = request.CountryName.Trim();
        city.IsActive = request.IsActive;
        city.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return await GetCityByIdAsync(city.Id)
            ?? throw new InvalidOperationException("City was updated but could not be loaded.");
    }

    public async Task DeleteCityAsync(Guid id)
    {
        var city = await _dbContext.Cities
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (city is null)
        {
            throw new KeyNotFoundException("City was not found.");
        }

        city.IsDeleted = true;
        city.DeletedAt = DateTime.UtcNow;
        city.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<CityResponse?> GetCityByIdAsync(Guid id)
    {
        return await _dbContext.Cities
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new CityResponse
            {
                Id = x.Id,
                Name = x.Name,
                StateName = x.StateName,
                CountryName = x.CountryName,
                IsActive = x.IsActive
            })
            .SingleOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<CityResponse>> GetCitiesAsync(bool? activeOnly = null)
    {
        var query = _dbContext.Cities
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (activeOnly.HasValue && activeOnly.Value)
        {
            query = query.Where(x => x.IsActive);
        }

        return await query
            .OrderBy(x => x.Name)
            .Select(x => new CityResponse
            {
                Id = x.Id,
                Name = x.Name,
                StateName = x.StateName,
                CountryName = x.CountryName,
                IsActive = x.IsActive
            })
            .ToArrayAsync();
    }
}

