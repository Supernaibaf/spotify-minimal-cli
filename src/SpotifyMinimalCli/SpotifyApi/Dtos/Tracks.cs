using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyApi.Dtos;

public class Tracks
{
    [JsonPropertyName("items")]
    public required IReadOnlyCollection<TrackObject> Items { get; init; }
}