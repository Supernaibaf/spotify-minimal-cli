using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyApi.Player;

public class SkipToNextRequest
{
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; init; }
}