using BookMyShow.Application.DTOs.Common;
using BookMyShow.Application.DTOs.Movies.Actors;
using BookMyShow.Application.Interfaces;
using BookMyShow.Domain.Entities.Movies;
using BookMyShow.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookMyShow.Infrastructure.Services;

public class ActorService : IActorService
{
    private readonly ApplicationDbContext _dbContext;

    public ActorService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ActorResponse> CreateActorAsync(CreateActorRequest request)
    {
        var nameTrimmed = request.FullName.Trim();
        var nameLower = nameTrimmed.ToLower();

        var existing = await _dbContext.Actors
            .SingleOrDefaultAsync(x => x.FullName.ToLower() == nameLower);

        if (existing is not null)
        {
            if (!existing.IsDeleted)
            {
                throw new InvalidOperationException("Actor with the same name already exists.");
            }
            else
            {
                existing.IsDeleted = false;
                existing.DeletedAt = null;
                existing.DeletedBy = null;
                existing.FullName = nameTrimmed;
                existing.ProfileImageUrl = string.IsNullOrWhiteSpace(request.ProfileImageUrl) ? null : request.ProfileImageUrl.Trim();
                existing.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                return await GetActorByIdAsync(existing.Id)
                    ?? throw new InvalidOperationException("Actor was restored but could not be loaded.");
            }
        }

        var actor = new Actor
        {
            FullName = nameTrimmed,
            ProfileImageUrl = string.IsNullOrWhiteSpace(request.ProfileImageUrl) ? null : request.ProfileImageUrl.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Actors.Add(actor);
        await _dbContext.SaveChangesAsync();

        return await GetActorByIdAsync(actor.Id)
            ?? throw new InvalidOperationException("Actor was created but could not be loaded.");
    }

    public async Task<ActorResponse> UpdateActorAsync(Guid id, UpdateActorRequest request)
    {
        var actor = await _dbContext.Actors
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (actor is null)
        {
            throw new KeyNotFoundException("Actor was not found.");
        }

        var nameTrimmed = request.FullName.Trim();
        var nameLower = nameTrimmed.ToLower();

        var duplicate = await _dbContext.Actors
            .SingleOrDefaultAsync(x => x.FullName.ToLower() == nameLower && x.Id != id);

        if (duplicate is not null)
        {
            if (!duplicate.IsDeleted)
            {
                throw new InvalidOperationException("Actor with the same name already exists.");
            }
            else
            {
                _dbContext.Actors.Remove(duplicate);
                await _dbContext.SaveChangesAsync();
            }
        }

        actor.FullName = nameTrimmed;
        actor.ProfileImageUrl = string.IsNullOrWhiteSpace(request.ProfileImageUrl) ? null : request.ProfileImageUrl.Trim();
        actor.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return await GetActorByIdAsync(actor.Id)
            ?? throw new InvalidOperationException("Actor was updated but could not be loaded.");
    }

    public async Task DeleteActorAsync(Guid id)
    {
        var actor = await _dbContext.Actors
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (actor is null)
        {
            throw new KeyNotFoundException("Actor was not found.");
        }

        actor.IsDeleted = true;
        actor.DeletedAt = DateTime.UtcNow;
        actor.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<ActorResponse?> GetActorByIdAsync(Guid id)
    {
        return await _dbContext.Actors
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new ActorResponse
            {
                Id = x.Id,
                FullName = x.FullName,
                ProfileImageUrl = x.ProfileImageUrl
            })
            .SingleOrDefaultAsync();
    }

    public async Task<PaginatedResponse<ActorListResponse>> GetActorsAsync(
        string? search,
        int pageNumber,
        int pageSize,
        string? sortBy,
        string? sortDirection)
    {
        var query = _dbContext.Actors
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim().ToLower();
            query = query.Where(x => x.FullName.ToLower().Contains(searchLower));
        }

        bool isDescending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        if (string.Equals(sortBy, "name", StringComparison.OrdinalIgnoreCase))
        {
            query = isDescending ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName);
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
            .Select(x => new ActorListResponse
            {
                Id = x.Id,
                FullName = x.FullName,
                ProfileImageUrl = x.ProfileImageUrl
            })
            .ToArrayAsync();

        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PaginatedResponse<ActorListResponse>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}

