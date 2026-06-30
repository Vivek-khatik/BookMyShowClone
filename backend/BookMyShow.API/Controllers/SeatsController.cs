using BookMyShow.Application.DTOs.Venues.Seats;
using BookMyShow.Application.Interfaces;
using BookMyShow.Domain.Entities.Venues;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookMyShow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SeatsController : ControllerBase
{
    private const string SeatWriteRoles = "SuperAdmin,Organizer";

    private readonly ISeatService _seatService;
    private readonly IScreenService _screenService;
    private readonly IValidator<CreateSeatRequest> _createSeatValidator;
    private readonly IValidator<UpdateSeatRequest> _updateSeatValidator;

    public SeatsController(
        ISeatService seatService,
        IScreenService screenService,
        IValidator<CreateSeatRequest> createSeatValidator,
        IValidator<UpdateSeatRequest> updateSeatValidator)
    {
        _seatService = seatService;
        _screenService = screenService;
        _createSeatValidator = createSeatValidator;
        _updateSeatValidator = updateSeatValidator;
    }

    [HttpPost]
    [Authorize(Roles = SeatWriteRoles)]
    public async Task<ActionResult<SeatResponse>> CreateSeat(CreateSeatRequest request)
    {
        var validationResult = await _createSeatValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var screen = await _screenService.GetScreenByIdAsync(request.ScreenId);
            if (screen != null)
            {
                var screenAvailableSeats = await _seatService.GetSeatCountByScreenId(request.ScreenId);
                if (screen.Capacity > screenAvailableSeats)
                {
                    var response = await _seatService.CreateSeatAsync(request);
                    return CreatedAtAction(nameof(GetSeatById), new { id = response.Id }, response);
                }
                else
                {
                    return BadRequest("Cannot add seats more than " + screen.Capacity + " in selected screen");
                }
            }
            else
            {
                return BadRequest("Screen not found");
            }
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("bulk/{screenId:guid}")]
    [Authorize(Roles = SeatWriteRoles)]
    public async Task<ActionResult<IReadOnlyCollection<SeatResponse>>> CreateSeatsBulk(Guid screenId, [FromBody] List<CreateSeatRequest> requests)
    {
        if (requests == null || requests.Count == 0)
        {
            return BadRequest(new { message = "Request body cannot be empty." });
        }

        foreach (var req in requests)
        {
            var validationResult = await _createSeatValidator.ValidateAsync(req);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
        }

        try
        {
            var screen = await _screenService.GetScreenByIdAsync(screenId);
            if (screen != null)
            {
                var screenAvailableSeats = await _seatService.GetSeatCountByScreenId(screenId);
                if (screen.Capacity > requests.Count && screenAvailableSeats > requests.Count)
                {
                    var response = await _seatService.CreateSeatsBulkAsync(screenId, requests);
                    return Ok(response);
                }
                else
                {
                    return BadRequest("Cannot add seats more than " + screen.Capacity + " in selected screen");
                }
            }
            else
            {
                return BadRequest("Screen not found");
            }
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = SeatWriteRoles)]
    public async Task<ActionResult<SeatResponse>> UpdateSeat(Guid id, UpdateSeatRequest request)
    {
        var validationResult = await _updateSeatValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _seatService.UpdateSeatAsync(id, request);
            return Ok(response);
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

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = SeatWriteRoles)]
    public async Task<IActionResult> DeleteSeat(Guid id)
    {
        try
        {
            await _seatService.DeleteSeatAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SeatResponse>> GetSeatById(Guid id)
    {
        var response = await _seatService.GetSeatByIdAsync(id);

        if (response is null)
        {
            return NotFound(new { message = "Seat was not found." });
        }

        return Ok(response);
    }

    [HttpGet("screen/{screenId:guid}")]
    public async Task<ActionResult<IReadOnlyCollection<SeatResponse>>> GetSeatsByScreen(Guid screenId)
    {
        var response = await _seatService.GetSeatsByScreenAsync(screenId);
        return Ok(response);
    }
}

