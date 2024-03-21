using System.Runtime.Serialization;

namespace SpotifyMinimalCli.SpotifyApi.Search;

public enum SearchType
{
    [EnumMember(Value = "track")]
    Track,
}