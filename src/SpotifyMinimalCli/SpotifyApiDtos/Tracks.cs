using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyApiDtos;

public class Tracks
{
    [JsonPropertyName("items")]
    public required IReadOnlyCollection<TrackObject> Items { get; init; }
}