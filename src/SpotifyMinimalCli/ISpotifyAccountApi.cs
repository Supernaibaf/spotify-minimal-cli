using Refit;
using SpotifyMinimalCli.SpotifyApiDtos;

namespace SpotifyMinimalCli;

public interface ISpotifyAccountApi
{
    [Post("/api/token")]
    Task<ApiResponse<TokenResponse>> RequestApiTokenAsync(
        [Body(BodySerializationMethod.UrlEncoded)]
        TokenRequest request);
}