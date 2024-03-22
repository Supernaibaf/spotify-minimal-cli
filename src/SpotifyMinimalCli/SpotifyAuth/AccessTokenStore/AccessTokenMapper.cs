using SpotifyMinimalCli.SpotifyAuth.AccountApi;

namespace SpotifyMinimalCli.SpotifyAuth.AccessTokenStore;

public static class AccessTokenMapper
{
    public static SpotifyAccessToken ToSpotifyAccessToken(
        this AccessTokenResponse accessTokenResponse,
        TimeProvider timeProvider)
    {
        return new SpotifyAccessToken
        {
            AccessToken = accessTokenResponse.AccessToken,
            TokenType = accessTokenResponse.TokenType,
            Scope = accessTokenResponse.Scope,
            ExpiresAt = CalculateExpiresAt(accessTokenResponse.ExpiresIn, timeProvider),
            RefreshToken = accessTokenResponse.RefreshToken,
        };
    }

    public static SpotifyAccessToken ToSpotifyAccessToken(
        this RefreshAccessTokenResponse refreshAccessTokenResponse,
        SpotifyAccessToken previousAccessToken,
        TimeProvider timeProvider)
    {
        return new SpotifyAccessToken
        {
            AccessToken = refreshAccessTokenResponse.AccessToken,
            TokenType = refreshAccessTokenResponse.TokenType,
            Scope = refreshAccessTokenResponse.Scope,
            ExpiresAt = CalculateExpiresAt(refreshAccessTokenResponse.ExpiresIn, timeProvider),
            RefreshToken = previousAccessToken.RefreshToken,
        };
    }

    private static DateTimeOffset CalculateExpiresAt(int expiresIn, TimeProvider timeProvider)
    {
        return timeProvider.GetUtcNow().Add(TimeSpan.FromSeconds(expiresIn));
    }
}