using BookMyShow.Application.DTOs.Common;
using BookMyShow.Application.DTOs.Movies.Actors;
using System.Threading.Tasks;

namespace BookMyShow.Application.Interfaces;

public interface IActorService
{
    Task<ActorResponse> CreateActorAsync(CreateActorRequest request);

    Task<ActorResponse> UpdateActorAsync(Guid id, UpdateActorRequest request);

    Task DeleteActorAsync(Guid id);

    Task<ActorResponse?> GetActorByIdAsync(Guid id);

    Task<PaginatedResponse<ActorListResponse>> GetActorsAsync(
        string? search,
        int pageNumber,
        int pageSize,
        string? sortBy,
        string? sortDirection);
}

