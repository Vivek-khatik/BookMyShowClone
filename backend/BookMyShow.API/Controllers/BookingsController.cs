using System.Security.Claims;
using BookMyShow.Application.DTOs.Bookings.Bookings;
using BookMyShow.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookMyShow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IValidator<CreateBookingRequest> _createBookingValidator;

    public BookingsController(
        IBookingService bookingService,
        IValidator<CreateBookingRequest> createBookingValidator)
    {
        _bookingService = bookingService;
        _createBookingValidator = createBookingValidator;
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponse>> CreateBooking(CreateBookingRequest request)
    {
        var validationResult = await _createBookingValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _bookingService.CreateBookingAsync(GetCurrentUserId(), request);
            return CreatedAtAction(nameof(GetBookingById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookingResponse>> GetBookingById(Guid id)
    {
        var response = await _bookingService.GetBookingByIdAsync(GetCurrentUserId(), id);

        if (response is null)
        {
            return NotFound(new { message = "Booking was not found." });
        }

        return Ok(response);
    }

    [HttpGet("my-bookings")]
    public async Task<ActionResult<IReadOnlyCollection<BookingSummaryResponse>>> GetMyBookings()
    {
        var response = await _bookingService.GetMyBookingsAsync(GetCurrentUserId());
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> CancelBooking(Guid id)
    {
        try
        {
            await _bookingService.CancelBookingAsync(GetCurrentUserId(), id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            throw new InvalidOperationException("Authenticated user identifier is invalid.");
        }

        return userId;
    }
}

