using BookMyShow.Application.DTOs.Venues.Venues;

namespace BookMyShow.Application.Interfaces;

public interface IVenueService
{
    Task<VenueResponse> CreateVenueAsync(CreateVenueRequest request);

    Task<VenueResponse> UpdateVenueAsync(Guid id, UpdateVenueRequest request);

    Task DeleteVenueAsync(Guid id);

    Task<VenueResponse?> GetVenueByIdAsync(Guid id);

    Task<IReadOnlyCollection<VenueListResponse>> GetVenuesAsync();

    Task<IReadOnlyCollection<VenueListResponse>> GetVenuesByCityAsync(Guid cityId);
}

