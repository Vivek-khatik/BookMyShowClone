using BookMyShow.Application.DTOs.Venues.Cities;
using BookMyShow.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookMyShow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private const string CityWriteRoles = "SuperAdmin,Organizer";

    private readonly ICityService _cityService;
    private readonly IValidator<CreateCityRequest> _createCityValidator;
    private readonly IValidator<UpdateCityRequest> _updateCityValidator;

    public CitiesController(
        ICityService cityService,
        IValidator<CreateCityRequest> createCityValidator,
        IValidator<UpdateCityRequest> updateCityValidator)
    {
        _cityService = cityService;
        _createCityValidator = createCityValidator;
        _updateCityValidator = updateCityValidator;
    }

    [HttpPost]
    [Authorize(Roles = CityWriteRoles)]
    public async Task<ActionResult<CityResponse>> CreateCity(CreateCityRequest request)
    {
        var validationResult = await _createCityValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _cityService.CreateCityAsync(request);
            return CreatedAtAction(nameof(GetCityById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = CityWriteRoles)]
    public async Task<ActionResult<CityResponse>> UpdateCity(Guid id, UpdateCityRequest request)
    {
        var validationResult = await _updateCityValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _cityService.UpdateCityAsync(id, request);
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
    [Authorize(Roles = CityWriteRoles)]
    public async Task<IActionResult> DeleteCity(Guid id)
    {
        try
        {
            await _cityService.DeleteCityAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CityResponse>> GetCityById(Guid id)
    {
        var response = await _cityService.GetCityByIdAsync(id);

        if (response is null)
        {
            return NotFound(new { message = "City was not found." });
        }

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CityResponse>>> GetCities([FromQuery] bool? activeOnly)
    {
        var response = await _cityService.GetCitiesAsync(activeOnly);
        return Ok(response);
    }
}

