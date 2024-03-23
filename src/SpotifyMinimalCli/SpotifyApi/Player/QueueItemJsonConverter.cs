using System.Text.Json;
using System.Text.Json.Serialization;
using SpotifyMinimalCli.SpotifyApi.Common;

namespace SpotifyMinimalCli.SpotifyApi.Player;

public class QueueItemJsonConverter : JsonConverter<QueueItem>
{
    public override QueueItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var type = doc.RootElement.TryGetProperty("type", out var typeElement)
            ? typeElement.GetString()
            : throw new JsonException($"Cannot find type for {nameof(QueueItem)}");

        return type switch
        {
            "track" => new QueueItem(doc.Deserialize<TrackObject>()),
            "episode" => new QueueItem(doc.Deserialize<EpisodeObject>()),
            _ => throw new JsonException($"Unknown type for {nameof(QueueItem)}: {type}"),
        };
    }

    public override void Write(Utf8JsonWriter writer, QueueItem value, JsonSerializerOptions options)
    {
        value.Switch(
            track => JsonSerializer.Serialize(writer, track, options),
            episode => JsonSerializer.Serialize(writer, episode, options));
    }
}