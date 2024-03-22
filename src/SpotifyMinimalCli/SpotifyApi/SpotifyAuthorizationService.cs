using System.Diagnostics;
using Microsoft.Extensions.Options;
using SpotifyMinimalCli.Authentication;
using SpotifyMinimalCli.SpotifyApi.Authorization;

namespace SpotifyMinimalCli.SpotifyApi;

public class SpotifyAuthorizationService(
    ISpotifyAccountApi spotifyAccountApi,
    IOptions<SpotifyAccountApiConfig> spotifyAccountApiConfig,
    IAuthenticationCallbackServer authenticationCallbackServer,
    IAccessTokenStore accessTokenStore)
    : ISpotifyAuthorizationService
{
    public async Task<Result<string, string>> GetAccessToken()
    {
        var storedAccessTokenResult = await accessTokenStore.LoadSpotifyAccessTokenAsync();
        if (storedAccessTokenResult.IsSuccess)
        {
            return Result.Success<string, string>(storedAccessTokenResult.Value.AccessToken);
        }

        var callbackCodeResult = await AuthorizeUser();
        if (!callbackCodeResult.IsSuccess)
        {
            return callbackCodeResult;
        }

        var newAccessTokenResult = await RequestAccessToken(callbackCodeResult.Value);
        if (!newAccessTokenResult.IsSuccess)
        {
            return Result.Failure<string, string>(newAccessTokenResult.Error);
        }

        var storeResult =
            await accessTokenStore.StoreSpotifyAccessTokenAsync(newAccessTokenResult.Value.ToSpotifyAccessToken());
        if (!storeResult.IsSuccess)
        {
            await Console.Error.WriteLineAsync(storeResult.Error);
        }

        return Result.Success<string, string>(newAccessTokenResult.Value.AccessToken);
    }

    private async Task<Result<string, string>> AuthorizeUser()
    {
        var state = Guid.NewGuid().ToString();
        var authenticationUrl = CreateAuthorizeUrl(state);

        Console.WriteLine($"For Login open the following Url in a browser: {authenticationUrl}");

        await OpenBrowser(authenticationUrl);

        var callbackCodeResult = await authenticationCallbackServer.WaitForCallbackCode(state);

        return callbackCodeResult.Map(success => success, error => $"Login failed: {error}");
    }

    private async Task<Result<AccessTokenResponse, string>> RequestAccessToken(string code)
    {
        var apiTokenResult = await spotifyAccountApi.RequestAccessTokenAsync(
            new AccessTokenRequest
            {
                GrantType = "authorization_code",
                Code = code,
                RedirectUri = authenticationCallbackServer.CallbackUrl,
            });

        if (apiTokenResult.IsSuccessStatusCode)
        {
            return apiTokenResult.Content;
        }

        return $"Access token request failed: {apiTokenResult.Error.GetSpotifyResponseErrorMessage()}";
    }

    private static async Task OpenBrowser(Uri authenticationUrl)
    {
        var process = Process.Start(
            new ProcessStartInfo
            {
                FileName = authenticationUrl.ToString(),
                UseShellExecute = true,
            });

        if (process != null)
        {
            await process.WaitForExitAsync();
        }
    }

    private Uri CreateAuthorizeUrl(string state)
    {
        var queryString = QueryString.Create(
            new KeyValuePair<string, string?>[]
            {
                new("client_id", spotifyAccountApiConfig.Value.ClientId),
                new("response_type", "code"),
                new("redirect_uri", authenticationCallbackServer.CallbackUrl.ToString()),
                new("state", state),
                new("scope", "user-modify-playback-state"),
                new("show_dialog", "true"),
            });

        return new Uri(
            spotifyAccountApiConfig.Value.BaseAddress,
            $"/authorize{queryString.ToUriComponent()}");
    }
}