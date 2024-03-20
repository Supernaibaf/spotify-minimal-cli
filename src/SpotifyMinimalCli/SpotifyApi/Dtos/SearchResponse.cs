using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyApi.Dtos;

public class SearchResponse
{
    [JsonPropertyName("tracks")]
    public Tracks? Tracks { get; init; }
}