using BookMyShow.Application.DTOs.Common;
using BookMyShow.Application.DTOs.Movies.Languages;
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
public class LanguagesController : ControllerBase
{
    private const string LanguageWriteRoles = "SuperAdmin,Organizer";

    private readonly ILanguageService _languageService;
    private readonly IValidator<CreateLanguageRequest> _createValidator;
    private readonly IValidator<UpdateLanguageRequest> _updateValidator;

    public LanguagesController(
        ILanguageService languageService,
        IValidator<CreateLanguageRequest> createValidator,
        IValidator<UpdateLanguageRequest> updateValidator)
    {
        _languageService = languageService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpPost]
    [Authorize(Roles = LanguageWriteRoles)]
    public async Task<ActionResult<LanguageResponse>> CreateLanguage(CreateLanguageRequest request)
    {
        var validationResult = await _createValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _languageService.CreateLanguageAsync(request);
            return CreatedAtAction(nameof(GetLanguageById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = LanguageWriteRoles)]
    public async Task<ActionResult<LanguageResponse>> UpdateLanguage(Guid id, UpdateLanguageRequest request)
    {
        var validationResult = await _updateValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _languageService.UpdateLanguageAsync(id, request);
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
    [Authorize(Roles = LanguageWriteRoles)]
    public async Task<IActionResult> DeleteLanguage(Guid id)
    {
        try
        {
            await _languageService.DeleteLanguageAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LanguageResponse>> GetLanguageById(Guid id)
    {
        var response = await _languageService.GetLanguageByIdAsync(id);

        if (response is null)
        {
            return NotFound(new { message = "Language was not found." });
        }

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<LanguageListResponse>>> GetLanguages(
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string sortBy = "name",
        [FromQuery] string sortDirection = "asc")
    {
        var response = await _languageService.GetLanguagesAsync(search, pageNumber, pageSize, sortBy, sortDirection);
        return Ok(response);
    }
}

