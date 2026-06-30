using System.Data;
using System.Security.Cryptography;
using BookMyShow.Application.DTOs.Bookings.Bookings;
using BookMyShow.Application.Interfaces;
using BookMyShow.Domain.Entities.Bookings;
using BookMyShow.Domain.Enums;
using BookMyShow.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMyShow.Infrastructure.Services;

public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _dbContext;

    public BookingService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BookingResponse> CreateBookingAsync(Guid userId, CreateBookingRequest request)
    {
        var seatIds = request.SeatIds.Distinct().ToArray();
        var utcNow = DateTime.UtcNow;

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var show = await _dbContext.Shows
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.ShowId && !x.IsDeleted);

        if (show is null)
        {
            throw new InvalidOperationException("Show was not found.");
        }

        var seats = await _dbContext.Seats
            .AsNoTracking()
            .Where(x =>
                x.ScreenId == show.ScreenId &&
                seatIds.Contains(x.Id) &&
                !x.IsDeleted)
            .Select(x => new
            {
                x.Id,
                x.RowName,
                x.SeatNumber,
                x.CategoryId,
                CategoryName = x.Category.Name
            })
            .ToArrayAsync();

        if (seats.Length != seatIds.Length)
        {
            throw new InvalidOperationException("One or more seats are invalid for the selected show.");
        }

        var alreadyBookedSeatIds = await _dbContext.BookingSeats
            .Where(x =>
                x.Booking.ShowId == request.ShowId &&
                seatIds.Contains(x.SeatId) &&
                !x.Booking.IsDeleted &&
                x.Booking.Status != BookingStatus.Cancelled &&
                x.Booking.Status != BookingStatus.Expired)
            .Select(x => x.SeatId)
            .Distinct()
            .ToArrayAsync();

        if (alreadyBookedSeatIds.Length > 0)
        {
            throw new InvalidOperationException($"Seats are already booked: {string.Join(", ", alreadyBookedSeatIds.OrderBy(x => x))}.");
        }

        var activeLocks = await _dbContext.SeatLocks
            .Where(x =>
                x.ShowId == request.ShowId &&
                seatIds.Contains(x.SeatId) &&
                !x.IsDeleted &&
                x.LockedUntil > utcNow)
            .ToArrayAsync();

        var conflictingLocks = activeLocks
            .Where(x => x.UserId != userId)
            .Select(x => x.SeatId)
            .Distinct()
            .ToArray();

        if (conflictingLocks.Length > 0)
        {
            throw new InvalidOperationException($"Seats are locked by another user: {string.Join(", ", conflictingLocks.OrderBy(x => x))}.");
        }

        var lockedByCurrentUserSeatIds = activeLocks
            .Where(x => x.UserId == userId)
            .Select(x => x.SeatId)
            .Distinct()
            .ToArray();

        var unlockedSeatIds = seatIds
            .Except(lockedByCurrentUserSeatIds)
            .ToArray();

        if (unlockedSeatIds.Length > 0)
        {
            throw new InvalidOperationException($"Seats must be locked before booking: {string.Join(", ", unlockedSeatIds.OrderBy(x => x))}.");
        }

        var prices = await _dbContext.ShowPrices
            .AsNoTracking()
            .Where(x => x.ShowId == request.ShowId && !x.IsDeleted)
            .ToDictionaryAsync(x => x.SeatCategoryId, x => x.Price);

        var missingPriceCategoryIds = seats
            .Select(x => x.CategoryId)
            .Distinct()
            .Where(categoryId => !prices.ContainsKey(categoryId))
            .ToArray();

        if (missingPriceCategoryIds.Length > 0)
        {
            throw new InvalidOperationException($"Pricing is missing for seat categories: {string.Join(", ", missingPriceCategoryIds.OrderBy(x => x))}.");
        }

        var totalAmount = seats.Sum(x => prices[x.CategoryId]);
        var bookingReference = await GenerateUniqueBookingReferenceAsync();

        var booking = new Booking
        {
            UserId = userId,
            ShowId = request.ShowId,
            BookingReference = bookingReference,
            TotalAmount = totalAmount,
            Status = BookingStatus.Confirmed,
            CreatedAt = utcNow,
            BookingSeats = seatIds
                .Select(seatId => new BookingSeat
                {
                    SeatId = seatId
                })
                .ToList()
        };

        _dbContext.Bookings.Add(booking);

        foreach (var seatLock in activeLocks.Where(x => x.UserId == userId))
        {
            seatLock.IsDeleted = true;
            seatLock.DeletedAt = utcNow;
            seatLock.UpdatedAt = utcNow;
        }

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return await GetBookingByIdAsync(userId, booking.Id)
            ?? throw new InvalidOperationException("Booking was created but could not be loaded.");
    }

    public async Task<BookingResponse?> GetBookingByIdAsync(Guid userId, Guid bookingId)
    {
        return await _dbContext.Bookings
            .AsNoTracking()
            .Where(x => x.Id == bookingId && x.UserId == userId && !x.IsDeleted)
            .Select(x => new BookingResponse
            {
                Id = x.Id,
                UserId = x.UserId,
                ShowId = x.ShowId,
                BookingReference = x.BookingReference,
                TotalAmount = x.TotalAmount,
                Status = x.Status,
                MovieTitle = x.Show.Movie != null ? x.Show.Movie.Title : null,
                ScreenName = x.Show.Screen.Name,
                VenueName = x.Show.Screen.Venue.Name,
                ShowStartTime = x.Show.StartTime,
                ShowEndTime = x.Show.EndTime,
                Seats = x.BookingSeats
                    .OrderBy(seat => seat.Seat.RowName)
                    .ThenBy(seat => seat.Seat.SeatNumber)
                    .Select(seat => new BookingSeatResponse
                    {
                        SeatId = seat.SeatId,
                        RowName = seat.Seat.RowName,
                        SeatNumber = seat.Seat.SeatNumber,
                        SeatCategoryId = seat.Seat.CategoryId,
                        SeatCategoryName = seat.Seat.Category.Name,
                        Price = x.Show.ShowPrices
                            .Where(price => !price.IsDeleted && price.SeatCategoryId == seat.Seat.CategoryId)
                            .Select(price => price.Price)
                            .FirstOrDefault()
                    })
                    .ToArray()
            })
            .SingleOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<BookingSummaryResponse>> GetMyBookingsAsync(Guid userId)
    {
        return await _dbContext.Bookings
            .AsNoTracking()
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new BookingSummaryResponse
            {
                Id = x.Id,
                BookingReference = x.BookingReference,
                ShowId = x.ShowId,
                MovieTitle = x.Show.Movie != null ? x.Show.Movie.Title : null,
                ScreenName = x.Show.Screen.Name,
                VenueName = x.Show.Screen.Venue.Name,
                ShowStartTime = x.Show.StartTime,
                TotalAmount = x.TotalAmount,
                Status = x.Status
            })
            .ToArrayAsync();
    }

    public async Task CancelBookingAsync(Guid userId, Guid bookingId)
    {
        var booking = await _dbContext.Bookings
            .SingleOrDefaultAsync(x => x.Id == bookingId && x.UserId == userId && !x.IsDeleted);

        if (booking is null)
        {
            throw new KeyNotFoundException("Booking was not found.");
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            return;
        }

        if (booking.Status == BookingStatus.Expired)
        {
            throw new InvalidOperationException("Expired bookings cannot be cancelled.");
        }

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    private async Task<string> GenerateUniqueBookingReferenceAsync()
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var candidate = $"BMS-{DateTime.UtcNow:yyyyMMdd}-{GenerateSuffix()}";
            var exists = await _dbContext.Bookings
                .AnyAsync(x => x.BookingReference == candidate);

            if (!exists)
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("Unable to generate a unique booking reference.");
    }

    private static string GenerateSuffix()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var bytes = RandomNumberGenerator.GetBytes(6);
        var buffer = new char[6];

        for (var i = 0; i < buffer.Length; i++)
        {
            buffer[i] = chars[bytes[i] % chars.Length];
        }

        return new string(buffer);
    }
}

