using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyApi.Search;

public class ArtistObject
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }
}