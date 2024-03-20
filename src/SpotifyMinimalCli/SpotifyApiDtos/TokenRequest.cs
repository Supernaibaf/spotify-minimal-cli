using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyApiDtos;

public class TokenRequest
{
    [JsonPropertyName("grant_type")]
    public required string GrantType { get; init; }

    [JsonPropertyName("client_id")]
    public required string ClientId { get; init; }

    [JsonPropertyName("client_secret")]
    public required string ClientSecret { get; init; }
}