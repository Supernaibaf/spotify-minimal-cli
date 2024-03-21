using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyApi.Player;

public class SkipToPreviousRequest
{
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; init; }
}