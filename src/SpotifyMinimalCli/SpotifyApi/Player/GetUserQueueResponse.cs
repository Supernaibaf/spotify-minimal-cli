using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyApi.Player;

public class GetUserQueueResponse
{
    [JsonPropertyName("currently_playing")]
    public QueueItem? CurrentlyPlaying { get; init; }

    [JsonPropertyName("queue")]
    public required IReadOnlyCollection<QueueItem> Queue { get; init; }
}