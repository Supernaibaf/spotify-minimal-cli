using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyApi.Authorization;

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    [JsonPropertyName("token_type")]
    public required string TokenType { get; init; }

    [JsonPropertyName("expires_in")]
    public required int ExpiresIn { get; init; }
}