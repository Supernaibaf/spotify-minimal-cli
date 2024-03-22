using Cocona;
using SpotifyMinimalCli.SpotifyApi;
using SpotifyMinimalCli.SpotifyApi.Player;

namespace SpotifyMinimalCli.Commands;

public static class PauseCommand
{
    public static void AddPauseCommand(this CoconaApp app)
    {
        _ = app.AddCommand("pause", Pause)
            .WithDescription("Pause playback");
    }

    private static async Task<int> Pause(ISpotifyApi spotifyApi, CoconaAppContext context)
    {
        var pauseResponse = await spotifyApi.PausePlaybackAsync(new PausePlaybackRequest(), context.CancellationToken);
        if (!pauseResponse.IsSuccessStatusCode)
        {
            await Console.Error.WriteLineAsync(
                $"Unable to pause playback: {pauseResponse.Error.GetSpotifyResponseErrorMessage()}");
            return -1;
        }

        return 0;
    }
}