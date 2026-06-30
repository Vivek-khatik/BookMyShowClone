using BookMyShow.Application.DTOs.Bookings.SeatLocks;

namespace BookMyShow.Application.Interfaces;

public interface ISeatLockService
{
    Task<LockSeatsResponse> LockSeatsAsync(Guid userId, LockSeatsRequest request);

    Task ReleaseSeatLockAsync(Guid userId, Guid seatLockId);

    Task<IReadOnlyCollection<AvailableSeatResponse>> GetAvailableSeatsAsync(Guid userId, Guid showId);
}

