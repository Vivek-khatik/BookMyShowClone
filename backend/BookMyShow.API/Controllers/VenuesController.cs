using BookMyShow.Application.DTOs.Venues.Venues;
using BookMyShow.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookMyShow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class VenuesController : ControllerBase
{
    private const string VenueWriteRoles = "SuperAdmin,Organizer";

    private readonly IVenueService _venueService;
    private readonly IValidator<CreateVenueRequest> _createVenueValidator;
    private readonly IValidator<UpdateVenueRequest> _updateVenueValidator;

    public VenuesController(
        IVenueService venueService,
        IValidator<CreateVenueRequest> createVenueValidator,
        IValidator<UpdateVenueRequest> updateVenueValidator)
    {
        _venueService = venueService;
        _createVenueValidator = createVenueValidator;
        _updateVenueValidator = updateVenueValidator;
    }

    [HttpPost]
    [Authorize(Roles = VenueWriteRoles)]
    public async Task<ActionResult<VenueResponse>> CreateVenue(CreateVenueRequest request)
    {
        var validationResult = await _createVenueValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _venueService.CreateVenueAsync(request);
            return CreatedAtAction(nameof(GetVenueById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = VenueWriteRoles)]
    public async Task<ActionResult<VenueResponse>> UpdateVenue(Guid id, UpdateVenueRequest request)
    {
        var validationResult = await _updateVenueValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _venueService.UpdateVenueAsync(id, request);
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
    [Authorize(Roles = VenueWriteRoles)]
    public async Task<IActionResult> DeleteVenue(Guid id)
    {
        try
        {
            await _venueService.DeleteVenueAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VenueResponse>> GetVenueById(Guid id)
    {
        var response = await _venueService.GetVenueByIdAsync(id);

        if (response is null)
        {
            return NotFound(new { message = "Venue was not found." });
        }

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<VenueListResponse>>> GetVenues()
    {
        var response = await _venueService.GetVenuesAsync();
        return Ok(response);
    }

    [HttpGet("city/{cityId:guid}")]
    public async Task<ActionResult<IReadOnlyCollection<VenueListResponse>>> GetVenuesByCity(Guid cityId)
    {
        var response = await _venueService.GetVenuesByCityAsync(cityId);
        return Ok(response);
    }
}

