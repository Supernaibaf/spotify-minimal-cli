using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyAuth.AccountApi;

public class AccessTokenResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    [JsonPropertyName("token_type")]
    public required string TokenType { get; init; }

    [JsonPropertyName("scope")]
    public required string Scope { get; init; }

    [JsonPropertyName("expires_in")]
    public required int ExpiresIn { get; init; }

    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; init; }
}