using System.Runtime.Serialization;

namespace SpotifyMinimalCli.SpotifyApi.Dtos;

public enum SearchType
{
    [EnumMember(Value = "track")]
    Track,
}