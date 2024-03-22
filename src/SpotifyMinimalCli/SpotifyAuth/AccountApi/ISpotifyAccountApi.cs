using Refit;

namespace SpotifyMinimalCli.SpotifyAuth.AccountApi;

public interface ISpotifyAccountApi
{
    [Post("/api/token")]
    Task<ApiResponse<AccessTokenResponse>> RequestAccessTokenAsync(
        [Body(BodySerializationMethod.UrlEncoded)]
        AccessTokenRequest request);

    [Post("/api/token")]
    Task<ApiResponse<RefreshAccessTokenResponse>> RefreshAccessTokenAsync(
        [Body(BodySerializationMethod.UrlEncoded)]
        RefreshAccessTokenRequest request);
}