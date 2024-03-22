using Refit;

namespace SpotifyMinimalCli.SpotifyApi.Player;

public class SeekToPositionRequest
{
    [AliasAs("position_ms")]
    public required int PositionMs { get; init; }

    [AliasAs("device_id")]
    public string? DeviceId { get; init; }
}