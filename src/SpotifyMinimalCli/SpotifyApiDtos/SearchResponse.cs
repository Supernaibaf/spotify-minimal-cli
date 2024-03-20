using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyApiDtos;

public class SearchResponse
{
    [JsonPropertyName("tracks")]
    public Tracks? Tracks { get; init; }
}