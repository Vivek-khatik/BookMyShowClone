using BookMyShow.Application.DTOs.Venues.SeatCategories;
using BookMyShow.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookMyShow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/seat-categories")]
public class SeatCategoriesController : ControllerBase
{
    private const string CategoryWriteRoles = "SuperAdmin,Organizer";

    private readonly ISeatCategoryService _categoryService;
    private readonly IValidator<CreateSeatCategoryRequest> _createCategoryValidator;
    private readonly IValidator<UpdateSeatCategoryRequest> _updateCategoryValidator;

    public SeatCategoriesController(
        ISeatCategoryService categoryService,
        IValidator<CreateSeatCategoryRequest> createCategoryValidator,
        IValidator<UpdateSeatCategoryRequest> updateCategoryValidator)
    {
        _categoryService = categoryService;
        _createCategoryValidator = createCategoryValidator;
        _updateCategoryValidator = updateCategoryValidator;
    }

    [HttpPost]
    [Authorize(Roles = CategoryWriteRoles)]
    public async Task<ActionResult<SeatCategoryResponse>> CreateCategory(CreateSeatCategoryRequest request)
    {
        var validationResult = await _createCategoryValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _categoryService.CreateSeatCategoryAsync(request);
            return CreatedAtAction(nameof(GetCategoryById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = CategoryWriteRoles)]
    public async Task<ActionResult<SeatCategoryResponse>> UpdateCategory(Guid id, UpdateSeatCategoryRequest request)
    {
        var validationResult = await _updateCategoryValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _categoryService.UpdateSeatCategoryAsync(id, request);
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
    [Authorize(Roles = CategoryWriteRoles)]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        try
        {
            await _categoryService.DeleteSeatCategoryAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SeatCategoryResponse>> GetCategoryById(Guid id)
    {
        var response = await _categoryService.GetSeatCategoryByIdAsync(id);

        if (response is null)
        {
            return NotFound(new { message = "Seat category was not found." });
        }

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<SeatCategoryResponse>>> GetCategories()
    {
        var response = await _categoryService.GetSeatCategoriesAsync();
        return Ok(response);
    }
}

