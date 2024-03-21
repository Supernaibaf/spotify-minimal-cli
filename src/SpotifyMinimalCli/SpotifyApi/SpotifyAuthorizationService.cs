using System.Diagnostics;
using Microsoft.Extensions.Options;
using SpotifyMinimalCli.AuthenticationCallback;
using SpotifyMinimalCli.SpotifyApi.Authorization;

namespace SpotifyMinimalCli.SpotifyApi;

public class SpotifyAuthorizationService(
    ISpotifyAccountApi spotifyAccountApi,
    IOptions<SpotifyAccountApiConfig> spotifyAccountApiConfig,
    IAuthenticationCallbackServer authenticationCallbackServer)
    : ISpotifyAuthorizationService
{
    public async Task<Result<string, string>> GetAccessToken()
    {
        var callbackCodeResult = await AuthorizeUser();
        if (!callbackCodeResult.IsSuccess)
        {
            return callbackCodeResult;
        }

        return await RequestAccessToken(callbackCodeResult.Value);
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

    private async Task<Result<string, string>> RequestAccessToken(string code)
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
            return Result.Success<string, string>(apiTokenResult.Content.AccessToken);
        }

        return Result.Failure<string, string>(
            $"Access token request failed: {apiTokenResult.Error.GetSpotifyResponseErrorMessage()}");
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