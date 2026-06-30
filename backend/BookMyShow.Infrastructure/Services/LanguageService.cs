using BookMyShow.Application.DTOs.Common;
using BookMyShow.Application.DTOs.Movies.Languages;
using BookMyShow.Application.Interfaces;
using BookMyShow.Domain.Entities.Movies;
using BookMyShow.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookMyShow.Infrastructure.Services;

public class LanguageService : ILanguageService
{
    private readonly ApplicationDbContext _dbContext;

    public LanguageService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LanguageResponse> CreateLanguageAsync(CreateLanguageRequest request)
    {
        var nameTrimmed = request.Name.Trim();
        var nameLower = nameTrimmed.ToLower();

        var existing = await _dbContext.Languages
            .SingleOrDefaultAsync(x => x.Name.ToLower() == nameLower);

        if (existing is not null)
        {
            if (!existing.IsDeleted)
            {
                throw new InvalidOperationException("Language with the same name already exists.");
            }
            else
            {
                existing.IsDeleted = false;
                existing.DeletedAt = null;
                existing.DeletedBy = null;
                existing.Name = nameTrimmed;
                existing.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                return await GetLanguageByIdAsync(existing.Id)
                    ?? throw new InvalidOperationException("Language was restored but could not be loaded.");
            }
        }

        var language = new Language
        {
            Name = nameTrimmed,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Languages.Add(language);
        await _dbContext.SaveChangesAsync();

        return await GetLanguageByIdAsync(language.Id)
            ?? throw new InvalidOperationException("Language was created but could not be loaded.");
    }

    public async Task<LanguageResponse> UpdateLanguageAsync(Guid id, UpdateLanguageRequest request)
    {
        var language = await _dbContext.Languages
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (language is null)
        {
            throw new KeyNotFoundException("Language was not found.");
        }

        var nameTrimmed = request.Name.Trim();
        var nameLower = nameTrimmed.ToLower();

        var duplicate = await _dbContext.Languages
            .SingleOrDefaultAsync(x => x.Name.ToLower() == nameLower && x.Id != id);

        if (duplicate is not null)
        {
            if (!duplicate.IsDeleted)
            {
                throw new InvalidOperationException("Language with the same name already exists.");
            }
            else
            {
                _dbContext.Languages.Remove(duplicate);
                await _dbContext.SaveChangesAsync();
            }
        }

        language.Name = nameTrimmed;
        language.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return await GetLanguageByIdAsync(language.Id)
            ?? throw new InvalidOperationException("Language was updated but could not be loaded.");
    }

    public async Task DeleteLanguageAsync(Guid id)
    {
        var language = await _dbContext.Languages
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (language is null)
        {
            throw new KeyNotFoundException("Language was not found.");
        }

        language.IsDeleted = true;
        language.DeletedAt = DateTime.UtcNow;
        language.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<LanguageResponse?> GetLanguageByIdAsync(Guid id)
    {
        return await _dbContext.Languages
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new LanguageResponse
            {
                Id = x.Id,
                Name = x.Name
            })
            .SingleOrDefaultAsync();
    }

    public async Task<PaginatedResponse<LanguageListResponse>> GetLanguagesAsync(
        string? search,
        int pageNumber,
        int pageSize,
        string? sortBy,
        string? sortDirection)
    {
        var query = _dbContext.Languages
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(searchLower));
        }

        bool isDescending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        if (string.Equals(sortBy, "name", StringComparison.OrdinalIgnoreCase))
        {
            query = isDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
        }
        else
        {
            query = query.OrderBy(x => x.Id);
        }

        int totalCount = await query.CountAsync();

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 20;

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new LanguageListResponse
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToArrayAsync();

        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PaginatedResponse<LanguageListResponse>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}

