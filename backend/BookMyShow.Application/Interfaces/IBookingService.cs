using BookMyShow.Application.DTOs.Bookings.Bookings;

namespace BookMyShow.Application.Interfaces;

public interface IBookingService
{
    Task<BookingResponse> CreateBookingAsync(Guid userId, CreateBookingRequest request);

    Task<BookingResponse?> GetBookingByIdAsync(Guid userId, Guid bookingId);

    Task<IReadOnlyCollection<BookingSummaryResponse>> GetMyBookingsAsync(Guid userId);

    Task CancelBookingAsync(Guid userId, Guid bookingId);
}

