using System.Diagnostics.CodeAnalysis;
using Refit;

namespace SpotifyMinimalCli.SpotifyApi.Player;

public class AddItemToPlaybackQueueRequest
{
    [AliasAs("uri")]
    [SuppressMessage(
        "Design",
        "CA1056:URI-like properties should not be strings",
        Justification = "Not a real Uri but spotify resource identifier")]
    public required string Uri { get; init; }

    [AliasAs("device_id")]
    public string? DeviceId { get; init; }
}