using Cocona;
using SpotifyMinimalCli.SpotifyApi;
using SpotifyMinimalCli.SpotifyApi.Player;

namespace SpotifyMinimalCli.Commands;

public static class PreviousCommand
{
    public static void AddPreviousCommand(this CoconaApp app)
    {
        _ = app.AddCommand("prev", Previous)
            .WithDescription("Skip to previous track in queue");
    }

    private static async Task<int> Previous(ISpotifyApi spotifyApi)
    {
        var previousResponse = await spotifyApi.PreviousAsync(new SkipToPreviousRequest());
        if (!previousResponse.IsSuccessStatusCode)
        {
            await Console.Error.WriteLineAsync(
                $"Unable to skip to previous track: {previousResponse.Error.GetSpotifyResponseErrorMessage()}");
            return -1;
        }

        return 0;
    }
}