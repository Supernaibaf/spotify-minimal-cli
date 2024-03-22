namespace SpotifyMinimalCli.SpotifyAuth.AccessTokenStore;

public interface ISpotifyAccessTokenStore
{
    Task<Result<SpotifyAccessToken, string>> LoadSpotifyAccessTokenAsync();

    Task<VoidResult<string>> StoreSpotifyAccessTokenAsync(SpotifyAccessToken spotifyAccessToken);
}