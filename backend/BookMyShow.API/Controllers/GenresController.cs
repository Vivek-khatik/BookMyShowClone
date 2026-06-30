using BookMyShow.Application.DTOs.Common;
using BookMyShow.Application.DTOs.Movies.Genres;
using BookMyShow.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookMyShow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private const string GenreWriteRoles = "SuperAdmin,Organizer";

    private readonly IGenreService _genreService;
    private readonly IValidator<CreateGenreRequest> _createValidator;
    private readonly IValidator<UpdateGenreRequest> _updateValidator;

    public GenresController(
        IGenreService genreService,
        IValidator<CreateGenreRequest> createValidator,
        IValidator<UpdateGenreRequest> updateValidator)
    {
        _genreService = genreService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpPost]
    [Authorize(Roles = GenreWriteRoles)]
    public async Task<ActionResult<GenreResponse>> CreateGenre(CreateGenreRequest request)
    {
        var validationResult = await _createValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _genreService.CreateGenreAsync(request);
            return CreatedAtAction(nameof(GetGenreById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = GenreWriteRoles)]
    public async Task<ActionResult<GenreResponse>> UpdateGenre(Guid id, UpdateGenreRequest request)
    {
        var validationResult = await _updateValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _genreService.UpdateGenreAsync(id, request);
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
    [Authorize(Roles = GenreWriteRoles)]
    public async Task<IActionResult> DeleteGenre(Guid id)
    {
        try
        {
            await _genreService.DeleteGenreAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GenreResponse>> GetGenreById(Guid id)
    {
        var response = await _genreService.GetGenreByIdAsync(id);

        if (response is null)
        {
            return NotFound(new { message = "Genre was not found." });
        }

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<GenreListResponse>>> GetGenres(
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string sortBy = "name",
        [FromQuery] string sortDirection = "asc")
    {
        var response = await _genreService.GetGenresAsync(search, pageNumber, pageSize, sortBy, sortDirection);
        return Ok(response);
    }
}

