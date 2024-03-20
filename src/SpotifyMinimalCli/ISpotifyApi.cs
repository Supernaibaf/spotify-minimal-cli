using Refit;
using SpotifyMinimalCli.SpotifyApiDtos;

namespace SpotifyMinimalCli;

public interface ISpotifyApi
{
    [Get("/v1/search")]
    Task<ApiResponse<SearchResponse>> SearchAsync([Query] SearchRequest request, [Authorize] string token);
}