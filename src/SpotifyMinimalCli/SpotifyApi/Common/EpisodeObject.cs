using System.Text.Json.Serialization;

namespace SpotifyMinimalCli.SpotifyApi.Common;

public class EpisodeObject
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonIgnore]
    public string DisplayValue => Name;
}