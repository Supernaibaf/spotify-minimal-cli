using Refit;

namespace SpotifyMinimalCli.SpotifyApi.Player;

public class PausePlaybackRequest
{
    [AliasAs("device_id")]
    public string? DeviceId { get; init; }
}