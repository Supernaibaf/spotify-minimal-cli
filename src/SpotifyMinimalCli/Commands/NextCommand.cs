using Cocona;
using SpotifyMinimalCli.SpotifyApi;
using SpotifyMinimalCli.SpotifyApi.Player;

namespace SpotifyMinimalCli.Commands;

public static class NextCommand
{
    public static void AddNextCommand(this CoconaApp app)
    {
        _ = app.AddCommand("next", Next)
            .WithDescription("Skip to next track in queue");
    }

    private static async Task<int> Next(ISpotifyApi spotifyApi, CoconaAppContext context)
    {
        var nextResponse = await spotifyApi.NextAsync(new SkipToNextRequest(), context.CancellationToken);
        if (!nextResponse.IsSuccessStatusCode)
        {
            await Console.Error.WriteLineAsync(
                $"Unable to skip to next track: {nextResponse.Error.GetSpotifyResponseErrorMessage()}");
            return -1;
        }

        return 0;
    }
}