using Refit;
using SpotifyMinimalCli.SpotifyApi.Player;
using SpotifyMinimalCli.SpotifyApi.Search;

namespace SpotifyMinimalCli.SpotifyApi;

public interface ISpotifyApi
{
    [Get("/v1/search")]
    Task<ApiResponse<SearchResponse>> SearchAsync([Query] SearchRequest request);

    [Post("/v1/me/player/next")]
    Task<IApiResponse> NextAsync([Query] SkipToNextRequest request);

    [Post("/v1/me/player/previous")]
    Task<IApiResponse> PreviousAsync([Query] SkipToPreviousRequest request);

    [Post("/v1/me/player/queue")]
    Task<IApiResponse> QueueTrackAsync([Query] AddItemToPlaybackQueueRequest request);
}