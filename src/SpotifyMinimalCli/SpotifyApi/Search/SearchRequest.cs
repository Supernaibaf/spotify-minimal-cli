using Refit;

namespace SpotifyMinimalCli.SpotifyApi.Search;

public class SearchRequest
{
    [AliasAs("q")]
    public required string Query { get; init; }

    [AliasAs("type")]
    public required SearchType Type { get; init; }

    [AliasAs("limit")]
    public required uint Limit { get; init; }
}