using System.Text.Json.Serialization;
using OneOf;
using SpotifyMinimalCli.SpotifyApi.Common;

namespace SpotifyMinimalCli.SpotifyApi.Player;

[GenerateOneOf]
[JsonConverter(typeof(QueueItemJsonConverter))]
public partial class QueueItem : OneOfBase<TrackObject, EpisodeObject>;