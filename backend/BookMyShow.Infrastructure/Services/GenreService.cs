using BookMyShow.Application.DTOs.Common;
using BookMyShow.Application.DTOs.Movies.Genres;
using BookMyShow.Application.Interfaces;
using BookMyShow.Domain.Entities.Movies;
using BookMyShow.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookMyShow.Infrastructure.Services;

public class GenreService : IGenreService
{
    private readonly ApplicationDbContext _dbContext;

    public GenreService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GenreResponse> CreateGenreAsync(CreateGenreRequest request)
    {
        var nameTrimmed = request.Name.Trim();
        var nameLower = nameTrimmed.ToLower();

        var existing = await _dbContext.Genres
            .SingleOrDefaultAsync(x => x.Name.ToLower() == nameLower);

        if (existing is not null)
        {
            if (!existing.IsDeleted)
            {
                throw new InvalidOperationException("Genre with the same name already exists.");
            }
            else
            {
                existing.IsDeleted = false;
                existing.DeletedAt = null;
                existing.DeletedBy = null;
                existing.Name = nameTrimmed;
                existing.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                return await GetGenreByIdAsync(existing.Id)
                    ?? throw new InvalidOperationException("Genre was restored but could not be loaded.");
            }
        }

        var genre = new Genre
        {
            Name = nameTrimmed,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Genres.Add(genre);
        await _dbContext.SaveChangesAsync();

        return await GetGenreByIdAsync(genre.Id)
            ?? throw new InvalidOperationException("Genre was created but could not be loaded.");
    }

    public async Task<GenreResponse> UpdateGenreAsync(Guid id, UpdateGenreRequest request)
    {
        var genre = await _dbContext.Genres
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (genre is null)
        {
            throw new KeyNotFoundException("Genre was not found.");
        }

        var nameTrimmed = request.Name.Trim();
        var nameLower = nameTrimmed.ToLower();

        var duplicate = await _dbContext.Genres
            .SingleOrDefaultAsync(x => x.Name.ToLower() == nameLower && x.Id != id);

        if (duplicate is not null)
        {
            if (!duplicate.IsDeleted)
            {
                throw new InvalidOperationException("Genre with the same name already exists.");
            }
            else
            {
                _dbContext.Genres.Remove(duplicate);
                await _dbContext.SaveChangesAsync();
            }
        }

        genre.Name = nameTrimmed;
        genre.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return await GetGenreByIdAsync(genre.Id)
            ?? throw new InvalidOperationException("Genre was updated but could not be loaded.");
    }

    public async Task DeleteGenreAsync(Guid id)
    {
        var genre = await _dbContext.Genres
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (genre is null)
        {
            throw new KeyNotFoundException("Genre was not found.");
        }

        genre.IsDeleted = true;
        genre.DeletedAt = DateTime.UtcNow;
        genre.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<GenreResponse?> GetGenreByIdAsync(Guid id)
    {
        return await _dbContext.Genres
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new GenreResponse
            {
                Id = x.Id,
                Name = x.Name
            })
            .SingleOrDefaultAsync();
    }

    public async Task<PaginatedResponse<GenreListResponse>> GetGenresAsync(
        string? search,
        int pageNumber,
        int pageSize,
        string? sortBy,
        string? sortDirection)
    {
        var query = _dbContext.Genres
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
            .Select(x => new GenreListResponse
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToArrayAsync();

        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PaginatedResponse<GenreListResponse>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}

