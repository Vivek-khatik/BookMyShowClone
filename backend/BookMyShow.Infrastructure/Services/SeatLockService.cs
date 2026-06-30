using System.Data;
using BookMyShow.Application.DTOs.Bookings.SeatLocks;
using BookMyShow.Application.Interfaces;
using BookMyShow.Domain.Entities.Bookings;
using BookMyShow.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMyShow.Infrastructure.Services;

public class SeatLockService : ISeatLockService
{
    private static readonly TimeSpan DefaultLockDuration = TimeSpan.FromMinutes(5);

    private readonly ApplicationDbContext _dbContext;

    public SeatLockService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LockSeatsResponse> LockSeatsAsync(Guid userId, LockSeatsRequest request)
    {
        var seatIds = request.SeatIds.Distinct().ToArray();
        var utcNow = DateTime.UtcNow;
        var lockedUntil = utcNow.Add(DefaultLockDuration);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var show = await _dbContext.Shows
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.ShowId && !x.IsDeleted);

        if (show is null)
        {
            throw new InvalidOperationException("Show was not found.");
        }

        var seatDetails = await GetValidShowSeatsAsync(show.ScreenId, seatIds);

        if (seatDetails.Count != seatIds.Length)
        {
            throw new InvalidOperationException("One or more seats are invalid for the selected show.");
        }

        var blockedSeatIds = await GetBlockedBookedSeatIdsAsync(request.ShowId, seatIds);

        if (blockedSeatIds.Length > 0)
        {
            throw new InvalidOperationException($"Seats are already booked: {FormatSeatIds(blockedSeatIds)}.");
        }

        var activeLocks = await _dbContext.SeatLocks
            .Where(x =>
                x.ShowId == request.ShowId &&
                seatIds.Contains(x.SeatId) &&
                !x.IsDeleted &&
                x.LockedUntil > utcNow)
            .ToArrayAsync();

        var conflictingLockSeatIds = activeLocks
            .Where(x => x.UserId != userId)
            .Select(x => x.SeatId)
            .Distinct()
            .ToArray();

        if (conflictingLockSeatIds.Length > 0)
        {
            throw new InvalidOperationException($"Seats are already locked: {FormatSeatIds(conflictingLockSeatIds)}.");
        }

        foreach (var existingLock in activeLocks.Where(x => x.UserId == userId))
        {
            existingLock.LockedUntil = lockedUntil;
            existingLock.UpdatedAt = utcNow;
        }

        var alreadyLockedByUserSeatIds = activeLocks
            .Where(x => x.UserId == userId)
            .Select(x => x.SeatId)
            .ToHashSet();

        var newLocks = seatDetails
            .Where(x => !alreadyLockedByUserSeatIds.Contains(x.SeatId))
            .Select(x => new SeatLock
            {
                ShowId = request.ShowId,
                SeatId = x.SeatId,
                UserId = userId,
                LockedUntil = lockedUntil,
                CreatedAt = utcNow
            })
            .ToList();

        if (newLocks.Count > 0)
        {
            _dbContext.SeatLocks.AddRange(newLocks);
        }

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        var responseLocks = await _dbContext.SeatLocks
            .AsNoTracking()
            .Where(x =>
                x.ShowId == request.ShowId &&
                x.UserId == userId &&
                seatIds.Contains(x.SeatId) &&
                !x.IsDeleted &&
                x.LockedUntil == lockedUntil)
            .Select(x => new SeatLockItemResponse
            {
                SeatLockId = x.Id,
                SeatId = x.SeatId,
                RowName = x.Seat.RowName,
                SeatNumber = x.Seat.SeatNumber,
                SeatCategoryName = x.Seat.Category.Name,
                LockedUntil = x.LockedUntil
            })
            .OrderBy(x => x.RowName)
            .ThenBy(x => x.SeatNumber)
            .ToArrayAsync();

        return new LockSeatsResponse
        {
            ShowId = request.ShowId,
            LockedUntil = lockedUntil,
            Seats = responseLocks
        };
    }

    public async Task ReleaseSeatLockAsync(Guid userId, Guid seatLockId)
    {
        var seatLock = await _dbContext.SeatLocks
            .SingleOrDefaultAsync(x => x.Id == seatLockId && !x.IsDeleted);

        if (seatLock is null)
        {
            throw new KeyNotFoundException("Seat lock was not found.");
        }

        if (seatLock.UserId != userId)
        {
            throw new InvalidOperationException("You can only release your own seat locks.");
        }

        seatLock.IsDeleted = true;
        seatLock.DeletedAt = DateTime.UtcNow;
        seatLock.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<AvailableSeatResponse>> GetAvailableSeatsAsync(Guid userId, Guid showId)
    {
        var utcNow = DateTime.UtcNow;

        var show = await _dbContext.Shows
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == showId && !x.IsDeleted);

        if (show is null)
        {
            throw new KeyNotFoundException("Show was not found.");
        }

        var bookedSeatIds = await GetBlockedBookedSeatIdsAsync(showId, null);

        var activeLocks = await _dbContext.SeatLocks
            .AsNoTracking()
            .Where(x =>
                x.ShowId == showId &&
                !x.IsDeleted &&
                x.LockedUntil > utcNow)
            .Select(x => new
            {
                x.SeatId,
                x.UserId,
                x.LockedUntil
            })
            .ToArrayAsync();

        var seats = await _dbContext.Seats
            .AsNoTracking()
            .Where(x => x.ScreenId == show.ScreenId && !x.IsDeleted)
            .Select(x => new
            {
                x.Id,
                x.RowName,
                x.SeatNumber,
                x.CategoryId,
                CategoryName = x.Category.Name
            })
            .ToArrayAsync();

        var priceMap = await _dbContext.ShowPrices
            .AsNoTracking()
            .Where(x => x.ShowId == showId && !x.IsDeleted)
            .ToDictionaryAsync(x => x.SeatCategoryId, x => x.Price);

        var availableSeats = seats
            .Where(seat => !bookedSeatIds.Contains(seat.Id))
            .Select(seat =>
            {
                var activeLock = activeLocks.FirstOrDefault(x => x.SeatId == seat.Id);
                var isLockedByOtherUser = activeLock is not null && activeLock.UserId != userId;

                return new
                {
                    Seat = seat,
                    ActiveLock = activeLock,
                    IsLockedByOtherUser = isLockedByOtherUser
                };
            })
            .Where(x => !x.IsLockedByOtherUser)
            .Where(x => priceMap.ContainsKey(x.Seat.CategoryId))
            .OrderBy(x => x.Seat.RowName)
            .ThenBy(x => x.Seat.SeatNumber)
            .Select(x => new AvailableSeatResponse
            {
                SeatId = x.Seat.Id,
                RowName = x.Seat.RowName,
                SeatNumber = x.Seat.SeatNumber,
                SeatCategoryId = x.Seat.CategoryId,
                SeatCategoryName = x.Seat.CategoryName,
                Price = priceMap[x.Seat.CategoryId],
                IsLockedByCurrentUser = x.ActiveLock is not null && x.ActiveLock.UserId == userId,
                LockedUntil = x.ActiveLock?.LockedUntil
            })
            .ToArray();

        return availableSeats;
    }

    private async Task<List<SeatValidationDetail>> GetValidShowSeatsAsync(Guid screenId, IReadOnlyCollection<Guid> seatIds)
    {
        return await _dbContext.Seats
            .AsNoTracking()
            .Where(x =>
                x.ScreenId == screenId &&
                seatIds.Contains(x.Id) &&
                !x.IsDeleted)
            .Select(x => new SeatValidationDetail
            {
                SeatId = x.Id,
                RowName = x.RowName,
                SeatNumber = x.SeatNumber,
                SeatCategoryName = x.Category.Name
            })
            .ToListAsync();
    }

    private async Task<Guid[]> GetBlockedBookedSeatIdsAsync(Guid showId, IReadOnlyCollection<Guid>? seatIds)
    {
        var query = _dbContext.BookingSeats
            .Where(x =>
                x.Booking.ShowId == showId &&
                !x.Booking.IsDeleted &&
                x.Booking.Status != Domain.Enums.BookingStatus.Cancelled &&
                x.Booking.Status != Domain.Enums.BookingStatus.Expired)
            .Select(x => x.SeatId);

        if (seatIds is not null)
        {
            query = query.Where(x => seatIds.Contains(x));
        }

        return await query.Distinct().ToArrayAsync();
    }

    private static string FormatSeatIds(IEnumerable<Guid> seatIds)
    {
        return string.Join(", ", seatIds.OrderBy(x => x));
    }

    private sealed class SeatValidationDetail
    {
        public Guid SeatId { get; set; }

        public string RowName { get; set; } = null!;

        public int SeatNumber { get; set; }

        public string SeatCategoryName { get; set; } = null!;
    }
}

