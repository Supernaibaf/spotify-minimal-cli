using Refit;
using SpotifyMinimalCli.SpotifyApi.Player;
using SpotifyMinimalCli.SpotifyApi.Search;

namespace SpotifyMinimalCli.SpotifyApi;

public interface ISpotifyApi
{
    [Get("/v1/search")]
    Task<ApiResponse<SearchResponse>> SearchAsync([Query] SearchRequest request, [Authorize] string token);

    [Post("/v1/me/player/next")]
    Task<IApiResponse> NextAsync([Query] SkipToNextRequest request, [Authorize] string token);

    [Post("/v1/me/player/previous")]
    Task<IApiResponse> PreviousAsync([Query] SkipToPreviousRequest request, [Authorize] string token);

    [Post("/v1/me/player/queue")]
    Task<IApiResponse> QueueTrackAsync([Query] AddItemToPlaybackQueueRequest request, [Authorize] string token);
}