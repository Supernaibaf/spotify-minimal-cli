using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyApi.Search;

public class TrackObject
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }
}