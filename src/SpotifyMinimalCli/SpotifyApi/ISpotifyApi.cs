using Refit;
using SpotifyMinimalCli.SpotifyApi.Player;
using SpotifyMinimalCli.SpotifyApi.Search;

namespace SpotifyMinimalCli.SpotifyApi;

public interface ISpotifyApi
{
    [Get("/v1/search")]
    Task<ApiResponse<SearchResponse>> SearchAsync([Query] SearchRequest request, CancellationToken cancellationToken);

    [Post("/v1/me/player/next")]
    Task<IApiResponse> NextAsync([Query] SkipToNextRequest request, CancellationToken cancellationToken);

    [Post("/v1/me/player/previous")]
    Task<IApiResponse> PreviousAsync([Query] SkipToPreviousRequest request, CancellationToken cancellationToken);

    [Post("/v1/me/player/queue")]
    Task<IApiResponse> QueueTrackAsync(
        [Query] AddItemToPlaybackQueueRequest request,
        CancellationToken cancellationToken);

    [Put("/v1/me/player/seek")]
    Task<IApiResponse> SeekToPositionAsync([Query] SeekToPositionRequest request, CancellationToken cancellationToken);
}