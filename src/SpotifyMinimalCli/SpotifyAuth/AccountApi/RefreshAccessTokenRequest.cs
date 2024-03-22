using Refit;

namespace SpotifyMinimalCli.SpotifyAuth.AccountApi;

public class RefreshAccessTokenRequest
{
    [AliasAs("grant_type")]
    public required string GrantType { get; init; }

    [AliasAs("refresh_token")]
    public required string RefreshToken { get; init; }
}