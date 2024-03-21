using Refit;
using SpotifyMinimalCli.SpotifyApi.Authorization;

namespace SpotifyMinimalCli.SpotifyApi;

public interface ISpotifyAccountApi
{
    [Post("/api/token")]
    Task<ApiResponse<TokenResponse>> RequestApiTokenAsync(
        [Body(BodySerializationMethod.UrlEncoded)]
        TokenRequest request);
}