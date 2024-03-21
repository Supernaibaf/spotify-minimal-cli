using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyApi;

public class SpotifyRegularErrorObject
{
    [JsonPropertyName("status")]
    public required int Status { get; init; }

    [JsonPropertyName("message")]
    public required string Message { get; init; }
}