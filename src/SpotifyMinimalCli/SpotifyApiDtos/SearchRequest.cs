using Refit;

namespace SpotifyMinimalCli.SpotifyApiDtos;

public class SearchRequest
{
    [AliasAs("q")]
    public required string Query { get; init; }

    [AliasAs("type")]
    public required SearchType Type { get; init; }

    [AliasAs("limit")]
    public required uint Limit { get; init; }
}