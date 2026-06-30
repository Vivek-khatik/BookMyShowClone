using BookMyShow.Application.DTOs.Venues.Seats;

namespace BookMyShow.Application.Interfaces;

public interface ISeatService
{
    Task<SeatResponse> CreateSeatAsync(CreateSeatRequest request);

    Task<IReadOnlyCollection<SeatResponse>> CreateSeatsBulkAsync(Guid screenId, IEnumerable<CreateSeatRequest> requests);

    Task<SeatResponse> UpdateSeatAsync(Guid id, UpdateSeatRequest request);

    Task DeleteSeatAsync(Guid id);

    Task<SeatResponse?> GetSeatByIdAsync(Guid id);

    Task<IReadOnlyCollection<SeatResponse>> GetSeatsByScreenAsync(Guid screenId);
    Task<int> GetSeatCountByScreenId(Guid screenId);
}

