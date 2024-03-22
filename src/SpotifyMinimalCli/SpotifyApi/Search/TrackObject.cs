using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyApi.Search;

public class TrackObject
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("uri")]
    [SuppressMessage(
        "Design",
        "CA1056:URI-like properties should not be strings",
        Justification = "Not a real Uri but spotify resource identifier")]
    public required string Uri { get; init; }

    [JsonPropertyName("artists")]
    public required IReadOnlyCollection<ArtistObject> Artists { get; init; }
}