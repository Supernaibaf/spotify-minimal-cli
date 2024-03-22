using Cocona;
using SpotifyMinimalCli.SpotifyApi;
using SpotifyMinimalCli.SpotifyApi.Player;

namespace SpotifyMinimalCli.Commands;

public static class StartCommand
{
    public static void AddStartCommand(this CoconaApp app)
    {
        _ = app.AddCommand("start", Start)
            .WithDescription("Start/Resume playback");
    }

    private static async Task<int> Start(ISpotifyApi spotifyApi, CoconaAppContext context)
    {
        var startResponse = await spotifyApi.StartPlaybackAsync(new StartPlaybackRequest(), context.CancellationToken);
        if (!startResponse.IsSuccessStatusCode)
        {
            await Console.Error.WriteLineAsync(
                $"Unable to start/resume playback: {startResponse.Error.GetSpotifyResponseErrorMessage()}");
            return -1;
        }

        return 0;
    }
}