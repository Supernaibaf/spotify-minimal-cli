using Cocona;
using SpotifyMinimalCli.SpotifyApi;
using SpotifyMinimalCli.SpotifyApi.Player;

namespace SpotifyMinimalCli.Commands;

public static class RestartCommand
{
    public static void AddRestartCommand(this CoconaApp app)
    {
        _ = app.AddCommand("restart", Restart)
            .WithDescription("Restart the current track");
    }

    private static async Task<int> Restart(ISpotifyApi spotifyApi, CoconaAppContext context)
    {
        var restartResponse = await spotifyApi.SeekToPositionAsync(
            new SeekToPositionRequest
            {
                PositionMs = 0,
            },
            context.CancellationToken);
        if (!restartResponse.IsSuccessStatusCode)
        {
            await Console.Error.WriteLineAsync(
                $"Unable to restart track: {restartResponse.Error.GetSpotifyResponseErrorMessage()}");
            return -1;
        }

        return 0;
    }
}