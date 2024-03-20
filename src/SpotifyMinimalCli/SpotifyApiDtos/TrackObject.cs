using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyApiDtos;

public class TrackObject
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }
}