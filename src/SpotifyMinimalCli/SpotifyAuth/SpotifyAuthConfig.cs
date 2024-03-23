namespace SpotifyMinimalCli.SpotifyAuth;

public class SpotifyAuthConfig
{
    public const string Key = "SpotifyAuth";

    public required Uri SpotifyAccountApiBaseAddress { get; init; }

    public required string Scopes { get; init; }

    public required string ClientId { get; init; }

    public required string ClientSecret { get; init; }
}