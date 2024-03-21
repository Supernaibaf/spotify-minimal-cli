using Refit;

namespace SpotifyMinimalCli.SpotifyApi.Authorization;

public class TokenRequest
{
    [AliasAs("grant_type")]
    public required string GrantType { get; init; }

    [AliasAs("client_id")]
    public required string ClientId { get; init; }

    [AliasAs("client_secret")]
    public required string ClientSecret { get; init; }
}