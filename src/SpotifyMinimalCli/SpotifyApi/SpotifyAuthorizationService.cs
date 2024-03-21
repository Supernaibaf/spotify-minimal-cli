using Microsoft.Extensions.Options;
using SpotifyMinimalCli.SpotifyApi.Authorization;

namespace SpotifyMinimalCli.SpotifyApi;

public class SpotifyAuthorizationService(
    IOptions<SpotifyAccountApiConfig> spotifyAccountApiConfig,
    ISpotifyAccountApi spotifyAccountApi)
    : ISpotifyAuthorizationService
{
    public async Task<Result<string, string>> GetAuthorizationToken()
    {
        var tokenResponse = await spotifyAccountApi.RequestApiTokenAsync(
            new TokenRequest
            {
                GrantType = spotifyAccountApiConfig.Value.GrantType,
                ClientId = spotifyAccountApiConfig.Value.ClientId,
                ClientSecret = spotifyAccountApiConfig.Value.ClientSecret,
            });

        return tokenResponse.IsSuccessStatusCode
            ? Result.Success<string, string>(tokenResponse.Content.AccessToken)
            : Result.Failure<string, string>($"Fetching authorization token failed: {tokenResponse.Error.Message}");
    }
}