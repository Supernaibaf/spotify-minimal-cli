using SpotifyMinimalCli.SpotifyAuth.AccountApi;

namespace SpotifyMinimalCli.SpotifyAuth.AccessTokenStore;

public static class AccessTokenMapper
{
    public static SpotifyAccessToken ToSpotifyAccessToken(this AccessTokenResponse accessTokenResponse)
    {
        return new SpotifyAccessToken
        {
            AccessToken = accessTokenResponse.AccessToken,
            TokenType = accessTokenResponse.TokenType,
            Scope = accessTokenResponse.Scope,
            ExpiresIn = accessTokenResponse.ExpiresIn,
            RefreshToken = accessTokenResponse.RefreshToken,
        };
    }
}