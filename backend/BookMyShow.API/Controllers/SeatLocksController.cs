using System.Security.Claims;
using BookMyShow.Application.DTOs.Bookings.SeatLocks;
using BookMyShow.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookMyShow.API.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public class SeatLocksController : ControllerBase
{
    private readonly ISeatLockService _seatLockService;
    private readonly IValidator<LockSeatsRequest> _lockSeatsValidator;

    public SeatLocksController(
        ISeatLockService seatLockService,
        IValidator<LockSeatsRequest> lockSeatsValidator)
    {
        _seatLockService = seatLockService;
        _lockSeatsValidator = lockSeatsValidator;
    }

    [HttpPost("seat-locks")]
    public async Task<ActionResult<LockSeatsResponse>> LockSeats(LockSeatsRequest request)
    {
        var validationResult = await _lockSeatsValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _seatLockService.LockSeatsAsync(GetCurrentUserId(), request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("seat-locks/{id:guid}")]
    public async Task<IActionResult> ReleaseSeatLock(Guid id)
    {
        try
        {
            await _seatLockService.ReleaseSeatLockAsync(GetCurrentUserId(), id);
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

    [HttpGet("shows/{showId:guid}/available-seats")]
    public async Task<ActionResult<IReadOnlyCollection<AvailableSeatResponse>>> GetAvailableSeats(Guid showId)
    {
        try
        {
            var response = await _seatLockService.GetAvailableSeatsAsync(GetCurrentUserId(), showId);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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

