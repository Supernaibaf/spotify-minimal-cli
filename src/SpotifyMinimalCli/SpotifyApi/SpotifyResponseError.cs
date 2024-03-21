using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyApi;

public class SpotifyResponseError
{
    [JsonPropertyName("error")]
    public required SpotifyRegularErrorObject Error { get; init; }
}