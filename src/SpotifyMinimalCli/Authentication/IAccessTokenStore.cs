namespace SpotifyMinimalCli.Authentication;

public interface IAccessTokenStore
{
    Task<Result<SpotifyAccessToken, string>> LoadSpotifyAccessTokenAsync();

    Task<VoidResult<string>> StoreSpotifyAccessTokenAsync(SpotifyAccessToken spotifyAccessToken);
}