namespace SpotifyMinimalCli.SpotifyApi;

public class SpotifyAccountApiConfig
{
    public const string Key = "SpotifyAccountApi";

    public required string BaseAddress { get; init; }

    public required string GrantType { get; init; }

    public required string ClientId { get; init; }

    public required string ClientSecret { get; init; }
}