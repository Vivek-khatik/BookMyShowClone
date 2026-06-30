using BookMyShow.Application.DTOs.Movies;
using BookMyShow.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookMyShow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private const string MovieWriteRoles = "SuperAdmin,Organizer";

    private readonly IMovieService _movieService;
    private readonly IValidator<CreateMovieRequest> _createMovieValidator;
    private readonly IValidator<UpdateMovieRequest> _updateMovieValidator;

    public MoviesController(
        IMovieService movieService,
        IValidator<CreateMovieRequest> createMovieValidator,
        IValidator<UpdateMovieRequest> updateMovieValidator)
    {
        _movieService = movieService;
        _createMovieValidator = createMovieValidator;
        _updateMovieValidator = updateMovieValidator;
    }

    [HttpPost]
    [Authorize(Roles = MovieWriteRoles)]
    public async Task<ActionResult<MovieResponse>> CreateMovie(CreateMovieRequest request)
    {
        var validationResult = await _createMovieValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _movieService.CreateMovieAsync(request);
            return CreatedAtAction(nameof(GetMovieById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = MovieWriteRoles)]
    public async Task<ActionResult<MovieResponse>> UpdateMovie(Guid id, UpdateMovieRequest request)
    {
        var validationResult = await _updateMovieValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _movieService.UpdateMovieAsync(id, request);
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
    [Authorize(Roles = MovieWriteRoles)]
    public async Task<IActionResult> DeleteMovie(Guid id)
    {
        try
        {
            await _movieService.DeleteMovieAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MovieResponse>> GetMovieById(Guid id)
    {
        var response = await _movieService.GetMovieByIdAsync(id);

        if (response is null)
        {
            return NotFound(new { message = "Movie was not found." });
        }

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<MovieListResponse>>> GetMovies()
    {
        var response = await _movieService.GetMoviesAsync();
        return Ok(response);
    }
}

