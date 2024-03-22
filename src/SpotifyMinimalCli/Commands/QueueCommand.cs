using Cocona;
using SpotifyMinimalCli.SpotifyApi;
using SpotifyMinimalCli.SpotifyApi.Player;
using SpotifyMinimalCli.SpotifyApi.Search;

namespace SpotifyMinimalCli.Commands;

public static class QueueCommand
{
    public static void AddQueueCommand(this CoconaApp builder)
    {
        _ = builder.AddCommand("queue", Queue)
            .WithDescription("Adds a track to the queue");
    }

    private static async Task<int> Queue(
        [Argument] string[] trackNames,
        ISpotifyAuthorizationService authorizationService,
        ISpotifyApi spotifyApi)
    {
        var tokenResult = await authorizationService.GetAccessToken();

        if (!tokenResult.IsSuccess)
        {
            await Console.Error.WriteLineAsync(tokenResult.Error);
            return -1;
        }

        foreach (var trackName in trackNames)
        {
            var searchResult = await SearchTrack(trackName, spotifyApi, tokenResult.Value);
            if (!searchResult.IsSuccess)
            {
                await Console.Error.WriteLineAsync(searchResult.Error);
                return -1;
            }

            var queueResult = await spotifyApi.QueueTrackAsync(
                new AddItemToPlaybackQueueRequest
                {
                    Uri = searchResult.Value.Uri,
                },
                tokenResult.Value);
            if (!queueResult.IsSuccessStatusCode)
            {
                await Console.Error.WriteLineAsync(
                    $"Unable to queue track \"{trackName}\": {queueResult.Error.GetSpotifyResponseErrorMessage()}");
                return -1;
            }

            Console.WriteLine($"Queued \"{searchResult.Value.Name}\"");
        }

        return 0;
    }

    private static async Task<Result<TrackObject, string>> SearchTrack(
        string trackName,
        ISpotifyApi spotifyApi,
        string token)
    {
        var searchResponse = await spotifyApi.SearchAsync(
            new SearchRequest
            {
                Query = trackName,
                Type = SearchType.Track,
                Limit = 1,
            },
            token);

        if (!searchResponse.IsSuccessStatusCode)
        {
            return $"Search for \"{trackName}\" failed: {searchResponse.Error.GetSpotifyResponseErrorMessage()}";
        }

        if (searchResponse.Content.Tracks == null || searchResponse.Content.Tracks.Items.Count == 0)
        {
            return $"Unable to find track with name \"{trackName}\"";
        }

        return searchResponse.Content.Tracks.Items.First();
    }
}