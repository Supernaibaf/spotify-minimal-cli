using Cocona;
using SpotifyMinimalCli.SpotifyApi;
using SpotifyMinimalCli.SpotifyApi.Dtos;

namespace SpotifyMinimalCli.Commands;

public static class QueueCommand
{
    public static void AddQueueCommand(this CoconaApp builder)
    {
        _ = builder.AddCommand("queue", Queue)
            .WithDescription("Adds a track to the queue");
    }

    private static async Task Queue(
        [Argument] string[] trackNames,
        ISpotifyAuthorizationService authorizationService,
        ISpotifyApi spotifyApi)
    {
        var tokenResult = await authorizationService.GetAuthorizationToken();

        if (!tokenResult.IsSuccess)
        {
            await Console.Error.WriteLineAsync(tokenResult.Error);
            return;
        }

        foreach (var trackName in trackNames)
        {
            var searchResult = await SearchTrack(trackName, spotifyApi, tokenResult.Value);
            if (searchResult.IsSuccess)
            {
                Console.WriteLine($"Queued \"{searchResult.Value.Name}\"");
            }
            else
            {
                await Console.Error.WriteLineAsync(searchResult.Error);
            }
        }
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
            return $"Search for \"{trackName}\" failed: {searchResponse.Error.Message}";
        }

        if (searchResponse.Content.Tracks == null || searchResponse.Content.Tracks.Items.Count == 0)
        {
            return $"Unable to find track with name \"{trackName}\"";
        }

        return searchResponse.Content.Tracks.Items.First();
    }
}