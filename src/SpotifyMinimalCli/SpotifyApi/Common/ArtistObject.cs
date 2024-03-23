using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyApi.Common;

public class ArtistObject
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }
}