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

    private static async Task<int> Previous(ISpotifyAuthorizationService authorizationService, ISpotifyApi spotifyApi)
    {
        var tokenResult = await authorizationService.GetAccessToken();

        if (!tokenResult.IsSuccess)
        {
            await Console.Error.WriteLineAsync(tokenResult.Error);
            return -1;
        }

        var previousResponse = await spotifyApi.PreviousAsync(new SkipToPreviousRequest(), tokenResult.Value);
        if (!previousResponse.IsSuccessStatusCode)
        {
            await Console.Error.WriteLineAsync(
                $"Unable to skip to previous track: {previousResponse.Error.GetSpotifyResponseErrorMessage()}");
            return -1;
        }

        return 0;
    }
}