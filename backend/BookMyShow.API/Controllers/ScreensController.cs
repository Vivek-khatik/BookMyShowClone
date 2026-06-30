using BookMyShow.Application.DTOs.Venues.Screens;
using BookMyShow.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookMyShow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ScreensController : ControllerBase
{
    private const string ScreenWriteRoles = "SuperAdmin,Organizer";

    private readonly IScreenService _screenService;
    private readonly IValidator<CreateScreenRequest> _createScreenValidator;
    private readonly IValidator<UpdateScreenRequest> _updateScreenValidator;

    public ScreensController(
        IScreenService screenService,
        IValidator<CreateScreenRequest> createScreenValidator,
        IValidator<UpdateScreenRequest> updateScreenValidator)
    {
        _screenService = screenService;
        _createScreenValidator = createScreenValidator;
        _updateScreenValidator = updateScreenValidator;
    }

    [HttpPost]
    [Authorize(Roles = ScreenWriteRoles)]
    public async Task<ActionResult<ScreenResponse>> CreateScreen(CreateScreenRequest request)
    {
        var validationResult = await _createScreenValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _screenService.CreateScreenAsync(request);
            return CreatedAtAction(nameof(GetScreenById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = ScreenWriteRoles)]
    public async Task<ActionResult<ScreenResponse>> UpdateScreen(Guid id, UpdateScreenRequest request)
    {
        var validationResult = await _updateScreenValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _screenService.UpdateScreenAsync(id, request);
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
    [Authorize(Roles = ScreenWriteRoles)]
    public async Task<IActionResult> DeleteScreen(Guid id)
    {
        try
        {
            await _screenService.DeleteScreenAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ScreenResponse>> GetScreenById(Guid id)
    {
        var response = await _screenService.GetScreenByIdAsync(id);

        if (response is null)
        {
            return NotFound(new { message = "Screen was not found." });
        }

        return Ok(response);
    }

    [HttpGet("venue/{venueId:guid}")]
    public async Task<ActionResult<IReadOnlyCollection<ScreenResponse>>> GetScreensByVenue(Guid venueId)
    {
        var response = await _screenService.GetScreensByVenueAsync(venueId);
        return Ok(response);
    }
}

