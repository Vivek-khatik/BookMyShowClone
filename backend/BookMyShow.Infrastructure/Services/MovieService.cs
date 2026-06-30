using BookMyShow.Application.DTOs.Movies;
using BookMyShow.Application.DTOs.Movies.Genres;
using BookMyShow.Application.DTOs.Movies.Languages;
using BookMyShow.Application.DTOs.Movies.Actors;
using BookMyShow.Application.Interfaces;
using BookMyShow.Domain.Entities.Movies;
using BookMyShow.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMyShow.Infrastructure.Services;

public class MovieService : IMovieService
{
    private readonly ApplicationDbContext _dbContext;

    public MovieService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MovieResponse> CreateMovieAsync(CreateMovieRequest request)
    {
        await ValidateMovieReferencesAsync(request.GenreId, request.LanguageId, request.ActorIds);

        var movie = new Movie
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            DurationMinutes = request.DurationMinutes,
            ReleaseDate = request.ReleaseDate,
            PosterUrl = NormalizeOptionalUrl(request.PosterUrl),
            BannerUrl = NormalizeOptionalUrl(request.BannerUrl),
            TrailerUrl = NormalizeOptionalUrl(request.TrailerUrl),
            CensorRating = request.CensorRating.Trim(),
            GenreId = request.GenreId,
            LanguageId = request.LanguageId,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow,
            MovieActors = request.ActorIds
                .Select(actorId => new MovieActor { ActorId = actorId })
                .ToList()
        };

        _dbContext.Movies.Add(movie);
        await _dbContext.SaveChangesAsync();

        return await GetMovieByIdAsync(movie.Id)
            ?? throw new InvalidOperationException("Movie was created but could not be loaded.");
    }

    public async Task<MovieResponse> UpdateMovieAsync(Guid id, UpdateMovieRequest request)
    {
        var movie = await _dbContext.Movies
            .Include(x => x.MovieActors)
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (movie is null)
        {
            throw new KeyNotFoundException("Movie was not found.");
        }

        await ValidateMovieReferencesAsync(request.GenreId, request.LanguageId, request.ActorIds);

        movie.Title = request.Title.Trim();
        movie.Description = request.Description.Trim();
        movie.DurationMinutes = request.DurationMinutes;
        movie.ReleaseDate = request.ReleaseDate;
        movie.PosterUrl = NormalizeOptionalUrl(request.PosterUrl);
        movie.BannerUrl = NormalizeOptionalUrl(request.BannerUrl);
        movie.TrailerUrl = NormalizeOptionalUrl(request.TrailerUrl);
        movie.CensorRating = request.CensorRating.Trim();
        movie.GenreId = request.GenreId;
        movie.LanguageId = request.LanguageId;
        movie.Status = request.Status;
        movie.UpdatedAt = DateTime.UtcNow;

        _dbContext.MovieActors.RemoveRange(movie.MovieActors);
        movie.MovieActors = request.ActorIds
            .Select(actorId => new MovieActor
            {
                MovieId = movie.Id,
                ActorId = actorId
            })
            .ToList();

        await _dbContext.SaveChangesAsync();

        return await GetMovieByIdAsync(movie.Id)
            ?? throw new InvalidOperationException("Movie was updated but could not be loaded.");
    }

    public async Task DeleteMovieAsync(Guid id)
    {
        var movie = await _dbContext.Movies
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (movie is null)
        {
            throw new KeyNotFoundException("Movie was not found.");
        }

        movie.IsDeleted = true;
        movie.DeletedAt = DateTime.UtcNow;
        movie.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<MovieResponse?> GetMovieByIdAsync(Guid id)
    {
        return await _dbContext.Movies
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new MovieResponse
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                DurationMinutes = x.DurationMinutes,
                ReleaseDate = x.ReleaseDate,
                PosterUrl = x.PosterUrl,
                BannerUrl = x.BannerUrl,
                TrailerUrl = x.TrailerUrl,
                CensorRating = x.CensorRating,
                Genre = new GenreResponse
                {
                    Id = x.Genre.Id,
                    Name = x.Genre.Name
                },
                Language = new LanguageResponse
                {
                    Id = x.Language.Id,
                    Name = x.Language.Name
                },
                Status = x.Status,
                Actors = x.MovieActors
                    .OrderBy(movieActor => movieActor.Actor.FullName)
                    .Select(movieActor => new ActorResponse
                    {
                        Id = movieActor.Actor.Id,
                        FullName = movieActor.Actor.FullName,
                        ProfileImageUrl = movieActor.Actor.ProfileImageUrl
                    })
                    .ToArray()
            })
            .SingleOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<MovieListResponse>> GetMoviesAsync()
    {
        return await _dbContext.Movies
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.ReleaseDate)
            .ThenBy(x => x.Title)
            .Select(x => new MovieListResponse
            {
                Id = x.Id,
                Title = x.Title,
                DurationMinutes = x.DurationMinutes,
                ReleaseDate = x.ReleaseDate,
                PosterUrl = x.PosterUrl,
                CensorRating = x.CensorRating,
                Genre = new GenreResponse
                {
                    Id = x.Genre.Id,
                    Name = x.Genre.Name
                },
                Language = new LanguageResponse
                {
                    Id = x.Language.Id,
                    Name = x.Language.Name
                },
                Status = x.Status
            })
            .ToArrayAsync();
    }

    private async Task ValidateMovieReferencesAsync(
        Guid genreId,
        Guid languageId,
        IReadOnlyCollection<Guid> actorIds)
    {
        var genreExists = await _dbContext.Genres
            .AnyAsync(x => x.Id == genreId && !x.IsDeleted);

        if (!genreExists)
        {
            throw new InvalidOperationException("Genre was not found.");
        }

        var languageExists = await _dbContext.Languages
            .AnyAsync(x => x.Id == languageId && !x.IsDeleted);

        if (!languageExists)
        {
            throw new InvalidOperationException("Language was not found.");
        }

        if (actorIds.Count == 0)
        {
            return;
        }

        var existingActorIds = await _dbContext.Actors
            .Where(x => actorIds.Contains(x.Id) && !x.IsDeleted)
            .Select(x => x.Id)
            .ToArrayAsync();

        var missingActorIds = actorIds
            .Except(existingActorIds)
            .ToArray();

        if (missingActorIds.Length > 0)
        {
            throw new InvalidOperationException($"Actors were not found: {string.Join(", ", missingActorIds)}.");
        }
    }

    private static string? NormalizeOptionalUrl(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

