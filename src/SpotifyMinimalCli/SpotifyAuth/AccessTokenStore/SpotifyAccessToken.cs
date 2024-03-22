namespace SpotifyMinimalCli.SpotifyAuth.AccessTokenStore;

public class SpotifyAccessToken
{
    public required string AccessToken { get; init; }

    public required string TokenType { get; init; }

    public required string Scope { get; init; }

    public required int ExpiresIn { get; init; }

    public required string RefreshToken { get; init; }
}