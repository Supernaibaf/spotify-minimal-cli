using Refit;
using SpotifyMinimalCli.SpotifyApi.Authorization;

namespace SpotifyMinimalCli.SpotifyApi;

public interface ISpotifyAccountApi
{
    [Post("/api/token")]
    Task<ApiResponse<AccessTokenResponse>> RequestAccessTokenAsync(
        [Body(BodySerializationMethod.UrlEncoded)]
        AccessTokenRequest request);
}