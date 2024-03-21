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

    private static async Task Next(ISpotifyAuthorizationService authorizationService, ISpotifyApi spotifyApi)
    {
        var tokenResult = await authorizationService.GetAuthorizationToken();

        if (!tokenResult.IsSuccess)
        {
            await Console.Error.WriteLineAsync(tokenResult.Error);
            return;
        }

        var nextResponse = await spotifyApi.NextAsync(new SkipToNextRequest(), tokenResult.Value);
        if (!nextResponse.IsSuccessStatusCode)
        {
            await Console.Error.WriteLineAsync($"Unable to skip to next track: {nextResponse.Error.GetSpotifyResponseErrorMessage()}");
        }
    }
}