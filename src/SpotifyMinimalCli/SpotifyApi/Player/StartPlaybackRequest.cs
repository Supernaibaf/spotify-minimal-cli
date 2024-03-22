using Refit;

namespace SpotifyMinimalCli.SpotifyApi.Player;

public class StartPlaybackRequest
{
    [AliasAs("device_id")]
    public string? DeviceId { get; init; }
}