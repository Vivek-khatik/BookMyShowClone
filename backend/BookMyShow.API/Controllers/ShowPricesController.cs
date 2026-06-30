using BookMyShow.Application.DTOs.Shows.ShowPrices;
using BookMyShow.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookMyShow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/show-prices")]
public class ShowPricesController : ControllerBase
{
    private const string ShowPriceWriteRoles = "SuperAdmin,Organizer";

    private readonly IShowPriceService _showPriceService;
    private readonly IValidator<CreateShowPriceRequest> _createShowPriceValidator;
    private readonly IValidator<UpdateShowPriceRequest> _updateShowPriceValidator;

    public ShowPricesController(
        IShowPriceService showPriceService,
        IValidator<CreateShowPriceRequest> createShowPriceValidator,
        IValidator<UpdateShowPriceRequest> updateShowPriceValidator)
    {
        _showPriceService = showPriceService;
        _createShowPriceValidator = createShowPriceValidator;
        _updateShowPriceValidator = updateShowPriceValidator;
    }

    [HttpPost]
    [Authorize(Roles = ShowPriceWriteRoles)]
    public async Task<ActionResult<ShowPriceResponse>> CreateShowPrice(CreateShowPriceRequest request)
    {
        var validationResult = await _createShowPriceValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _showPriceService.CreateShowPriceAsync(request);
            return CreatedAtAction(nameof(GetShowPrices), new { showId = response.ShowId }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = ShowPriceWriteRoles)]
    public async Task<ActionResult<ShowPriceResponse>> UpdateShowPrice(Guid id, UpdateShowPriceRequest request)
    {
        var validationResult = await _updateShowPriceValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _showPriceService.UpdateShowPriceAsync(id, request);
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
    [Authorize(Roles = ShowPriceWriteRoles)]
    public async Task<IActionResult> DeleteShowPrice(Guid id)
    {
        try
        {
            await _showPriceService.DeleteShowPriceAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("show/{showId:guid}")]
    public async Task<ActionResult<IReadOnlyCollection<ShowPriceResponse>>> GetShowPrices(Guid showId)
    {
        try
        {
            var response = await _showPriceService.GetShowPricesAsync(showId);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

