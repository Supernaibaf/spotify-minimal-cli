using Refit;
using SpotifyMinimalCli.SpotifyApi.Search;

namespace SpotifyMinimalCli.SpotifyApi;

public interface ISpotifyApi
{
    [Get("/v1/search")]
    Task<ApiResponse<SearchResponse>> SearchAsync([Query] SearchRequest request, [Authorize] string token);
}