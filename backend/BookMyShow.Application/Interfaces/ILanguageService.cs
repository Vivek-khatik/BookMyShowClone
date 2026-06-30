using BookMyShow.Application.DTOs.Common;
using BookMyShow.Application.DTOs.Movies.Languages;
using System.Threading.Tasks;

namespace BookMyShow.Application.Interfaces;

public interface ILanguageService
{
    Task<LanguageResponse> CreateLanguageAsync(CreateLanguageRequest request);

    Task<LanguageResponse> UpdateLanguageAsync(Guid id, UpdateLanguageRequest request);

    Task DeleteLanguageAsync(Guid id);

    Task<LanguageResponse?> GetLanguageByIdAsync(Guid id);

    Task<PaginatedResponse<LanguageListResponse>> GetLanguagesAsync(
        string? search,
        int pageNumber,
        int pageSize,
        string? sortBy,
        string? sortDirection);
}

