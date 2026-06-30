using BookMyShow.Application.DTOs.Venues.Seats;
using BookMyShow.Application.Interfaces;
using BookMyShow.Domain.Entities.Venues;
using BookMyShow.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMyShow.Infrastructure.Services;

public class SeatService : ISeatService
{
    private readonly ApplicationDbContext _dbContext;

    public SeatService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SeatResponse> CreateSeatAsync(CreateSeatRequest request)
    {
        await ValidateSeatReferencesAsync(request.ScreenId, request.CategoryId);
        await ValidateNoDuplicateSeatAsync(request.ScreenId, request.RowName, request.SeatNumber);

        var seat = new Seat
        {
            ScreenId = request.ScreenId,
            RowName = request.RowName.Trim().ToUpper(),
            SeatNumber = request.SeatNumber,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Seats.Add(seat);
        await _dbContext.SaveChangesAsync();

        return await GetSeatByIdAsync(seat.Id)
            ?? throw new InvalidOperationException("Seat was created but could not be loaded.");
    }

    public async Task<IReadOnlyCollection<SeatResponse>> CreateSeatsBulkAsync(Guid screenId, IEnumerable<CreateSeatRequest> requests)
    {
        var screenExists = await _dbContext.Screens
            .AnyAsync(x => x.Id == screenId && !x.IsDeleted);

        if (!screenExists)
        {
            throw new InvalidOperationException("Screen was not found.");
        }

        var seatList = new List<Seat>();
        var seenSeatsInRequest = new HashSet<(string RowName, int SeatNumber)>();

        foreach (var req in requests)
        {
            if (req.ScreenId != screenId)
            {
                throw new InvalidOperationException("Bulk seat creation requests must match the screen ID in route.");
            }

            var rowUpper = req.RowName.Trim().ToUpper();
            if (seenSeatsInRequest.Contains((rowUpper, req.SeatNumber)))
            {
                throw new InvalidOperationException($"Duplicate seat in request: Row {rowUpper}, Seat {req.SeatNumber}.");
            }
            seenSeatsInRequest.Add((rowUpper, req.SeatNumber));

            await ValidateSeatReferencesAsync(req.ScreenId, req.CategoryId);
            await ValidateNoDuplicateSeatAsync(req.ScreenId, req.RowName, req.SeatNumber);

            var seat = new Seat
            {
                ScreenId = req.ScreenId,
                RowName = rowUpper,
                SeatNumber = req.SeatNumber,
                CategoryId = req.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            seatList.Add(seat);
        }

        if (seatList.Count > 0)
        {
            _dbContext.Seats.AddRange(seatList);
            await _dbContext.SaveChangesAsync();
        }

        var seatIds = seatList.Select(x => x.Id).ToArray();

        return await _dbContext.Seats
            .AsNoTracking()
            .Where(x => seatIds.Contains(x.Id))
            .OrderBy(x => x.RowName)
            .ThenBy(x => x.SeatNumber)
            .Select(x => new SeatResponse
            {
                Id = x.Id,
                ScreenId = x.ScreenId,
                ScreenName = x.Screen.Name,
                RowName = x.RowName,
                SeatNumber = x.SeatNumber,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name
            })
            .ToArrayAsync();
    }

    public async Task<SeatResponse> UpdateSeatAsync(Guid id, UpdateSeatRequest request)
    {
        var seat = await _dbContext.Seats
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (seat is null)
        {
            throw new KeyNotFoundException("Seat was not found.");
        }

        var categoryExists = await _dbContext.SeatCategories
            .AnyAsync(x => x.Id == request.CategoryId && !x.IsDeleted);

        if (!categoryExists)
        {
            throw new InvalidOperationException("Seat category was not found.");
        }

        var rowUpper = request.RowName.Trim().ToUpper();
        if (seat.RowName != rowUpper || seat.SeatNumber != request.SeatNumber)
        {
            await ValidateNoDuplicateSeatAsync(seat.ScreenId, rowUpper, request.SeatNumber, id);
        }

        seat.RowName = rowUpper;
        seat.SeatNumber = request.SeatNumber;
        seat.CategoryId = request.CategoryId;
        seat.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return await GetSeatByIdAsync(seat.Id)
            ?? throw new InvalidOperationException("Seat was updated but could not be loaded.");
    }

    public async Task DeleteSeatAsync(Guid id)
    {
        var seat = await _dbContext.Seats
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (seat is null)
        {
            throw new KeyNotFoundException("Seat was not found.");
        }

        seat.IsDeleted = true;
        seat.DeletedAt = DateTime.UtcNow;
        seat.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<SeatResponse?> GetSeatByIdAsync(Guid id)
    {
        return await _dbContext.Seats
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new SeatResponse
            {
                Id = x.Id,
                ScreenId = x.ScreenId,
                ScreenName = x.Screen.Name,
                RowName = x.RowName,
                SeatNumber = x.SeatNumber,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name
            })
            .SingleOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<SeatResponse>> GetSeatsByScreenAsync(Guid screenId)
    {
        return await _dbContext.Seats
            .AsNoTracking()
            .Where(x => x.ScreenId == screenId && !x.IsDeleted)
            .OrderBy(x => x.RowName)
            .ThenBy(x => x.SeatNumber)
            .Select(x => new SeatResponse
            {
                Id = x.Id,
                ScreenId = x.ScreenId,
                ScreenName = x.Screen.Name,
                RowName = x.RowName,
                SeatNumber = x.SeatNumber,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name
            })
            .ToArrayAsync();
    }

    private async Task ValidateSeatReferencesAsync(Guid screenId, Guid categoryId)
    {
        var screenExists = await _dbContext.Screens
            .AnyAsync(x => x.Id == screenId && !x.IsDeleted);

        if (!screenExists)
        {
            throw new InvalidOperationException("Screen was not found.");
        }

        var categoryExists = await _dbContext.SeatCategories
            .AnyAsync(x => x.Id == categoryId && !x.IsDeleted);

        if (!categoryExists)
        {
            throw new InvalidOperationException("Seat category was not found.");
        }
    }

    private async Task ValidateNoDuplicateSeatAsync(Guid screenId, string rowName, int seatNumber, Guid? excludeSeatId = null)
    {
        var rowUpper = rowName.Trim().ToUpper();
        var query = _dbContext.Seats
            .Where(x => x.ScreenId == screenId && x.RowName == rowUpper && x.SeatNumber == seatNumber && !x.IsDeleted);

        if (excludeSeatId.HasValue)
        {
            query = query.Where(x => x.Id != excludeSeatId.Value);
        }

        var exists = await query.AnyAsync();

        if (exists)
        {
            throw new InvalidOperationException($"Seat {rowUpper}-{seatNumber} already exists in this screen.");
        }
    }

    public async Task<int> GetSeatCountByScreenId(Guid screenId)
    {
        var query = _dbContext.Seats.Where(x => x.ScreenId == screenId && !x.IsDeleted);
        var screenCount = await query.CountAsync();

        if (screenCount > 0)
        {
            return query.Count();
        }
        return 0;
    }
}

