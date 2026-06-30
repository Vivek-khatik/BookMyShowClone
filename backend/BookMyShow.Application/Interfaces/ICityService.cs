using BookMyShow.Application.DTOs.Venues.Cities;

namespace BookMyShow.Application.Interfaces;

public interface ICityService
{
    Task<CityResponse> CreateCityAsync(CreateCityRequest request);

    Task<CityResponse> UpdateCityAsync(Guid id, UpdateCityRequest request);

    Task DeleteCityAsync(Guid id);

    Task<CityResponse?> GetCityByIdAsync(Guid id);

    Task<IReadOnlyCollection<CityResponse>> GetCitiesAsync(bool? activeOnly = null);
}

