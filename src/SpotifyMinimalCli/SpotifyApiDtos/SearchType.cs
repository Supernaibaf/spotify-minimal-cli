using System.Runtime.Serialization;

namespace SpotifyMinimalCli.SpotifyApiDtos;

public enum SearchType
{
    [EnumMember(Value = "track")]
    Track,
}