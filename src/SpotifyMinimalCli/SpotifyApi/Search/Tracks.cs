using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyApi.Search;

public class Tracks
{
    [JsonPropertyName("items")]
    public required IReadOnlyCollection<TrackObject> Items { get; init; }
}