using System.Text.Json.Serialization;
using SpotifyMinimalCli.SpotifyApi.Common;

namespace SpotifyMinimalCli.SpotifyApi.Search;

public class Tracks
{
    [JsonPropertyName("items")]
    public required IReadOnlyCollection<TrackObject> Items { get; init; }
}