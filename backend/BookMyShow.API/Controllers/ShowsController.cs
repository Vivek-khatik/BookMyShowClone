using BookMyShow.Application.DTOs.Shows.Shows;
using BookMyShow.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookMyShow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ShowsController : ControllerBase
{
    private const string ShowWriteRoles = "SuperAdmin,Organizer";

    private readonly IShowService _showService;
    private readonly IValidator<CreateShowRequest> _createShowValidator;
    private readonly IValidator<UpdateShowRequest> _updateShowValidator;

    public ShowsController(
        IShowService showService,
        IValidator<CreateShowRequest> createShowValidator,
        IValidator<UpdateShowRequest> updateShowValidator)
    {
        _showService = showService;
        _createShowValidator = createShowValidator;
        _updateShowValidator = updateShowValidator;
    }

    [HttpPost]
    [Authorize(Roles = ShowWriteRoles)]
    public async Task<ActionResult<ShowResponse>> CreateShow(CreateShowRequest request)
    {
        var validationResult = await _createShowValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _showService.CreateShowAsync(request);
            return CreatedAtAction(nameof(GetShowById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = ShowWriteRoles)]
    public async Task<ActionResult<ShowResponse>> UpdateShow(Guid id, UpdateShowRequest request)
    {
        var validationResult = await _updateShowValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _showService.UpdateShowAsync(id, request);
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
    [Authorize(Roles = ShowWriteRoles)]
    public async Task<IActionResult> DeleteShow(Guid id)
    {
        try
        {
            await _showService.DeleteShowAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ShowResponse>> GetShowById(Guid id)
    {
        var response = await _showService.GetShowByIdAsync(id);

        if (response is null)
        {
            return NotFound(new { message = "Show was not found." });
        }

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ShowListResponse>>> GetShows()
    {
        var response = await _showService.GetShowsAsync();
        return Ok(response);
    }
}

