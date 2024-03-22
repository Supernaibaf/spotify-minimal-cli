using Refit;

namespace SpotifyMinimalCli.SpotifyAuth.AccountApi;

public interface ISpotifyAccountApi
{
    [Post("/api/token")]
    Task<ApiResponse<AccessTokenResponse>> RequestAccessTokenAsync(
        [Body(BodySerializationMethod.UrlEncoded)]
        AccessTokenRequest request);
}