using BookMyShow.Application.DTOs.Common;
using BookMyShow.Application.DTOs.Movies.Actors;
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
public class ActorsController : ControllerBase
{
    private const string ActorWriteRoles = "SuperAdmin,Organizer";

    private readonly IActorService _actorService;
    private readonly IValidator<CreateActorRequest> _createValidator;
    private readonly IValidator<UpdateActorRequest> _updateValidator;

    public ActorsController(
        IActorService actorService,
        IValidator<CreateActorRequest> createValidator,
        IValidator<UpdateActorRequest> updateValidator)
    {
        _actorService = actorService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpPost]
    [Authorize(Roles = ActorWriteRoles)]
    public async Task<ActionResult<ActorResponse>> CreateActor(CreateActorRequest request)
    {
        var validationResult = await _createValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _actorService.CreateActorAsync(request);
            return CreatedAtAction(nameof(GetActorById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = ActorWriteRoles)]
    public async Task<ActionResult<ActorResponse>> UpdateActor(Guid id, UpdateActorRequest request)
    {
        var validationResult = await _updateValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _actorService.UpdateActorAsync(id, request);
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
    [Authorize(Roles = ActorWriteRoles)]
    public async Task<IActionResult> DeleteActor(Guid id)
    {
        try
        {
            await _actorService.DeleteActorAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ActorResponse>> GetActorById(Guid id)
    {
        var response = await _actorService.GetActorByIdAsync(id);

        if (response is null)
        {
            return NotFound(new { message = "Actor was not found." });
        }

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<ActorListResponse>>> GetActors(
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string sortBy = "name",
        [FromQuery] string sortDirection = "asc")
    {
        var response = await _actorService.GetActorsAsync(search, pageNumber, pageSize, sortBy, sortDirection);
        return Ok(response);
    }
}

