using Refit;

namespace SpotifyMinimalCli.SpotifyApi.Authorization;

public class AccessTokenRequest
{
    [AliasAs("grant_type")]
    public required string GrantType { get; init; }

    [AliasAs("code")]
    public required string Code { get; init; }

    [AliasAs("redirect_uri")]
    public required Uri RedirectUri { get; init; }
}