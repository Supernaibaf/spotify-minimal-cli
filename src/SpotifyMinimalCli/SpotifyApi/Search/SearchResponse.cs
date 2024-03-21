using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyApi.Search;

public class SearchResponse
{
    [JsonPropertyName("tracks")]
    public Tracks? Tracks { get; init; }
}