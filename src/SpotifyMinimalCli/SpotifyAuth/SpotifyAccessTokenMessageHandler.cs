using System.Diagnostics;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using SpotifyMinimalCli.SpotifyAuth.AccessTokenStore;
using SpotifyMinimalCli.SpotifyAuth.AccountApi;
using SpotifyMinimalCli.SpotifyAuth.AuthCallback;

namespace SpotifyMinimalCli.SpotifyAuth;

public class SpotifyAccessTokenMessageHandler(
    ISpotifyAccountApi spotifyAccountApi,
    IOptions<SpotifyAccountApiConfig> spotifyAccountApiConfig,
    IAuthenticationCallbackServer authenticationCallbackServer,
    ISpotifyAccessTokenStore spotifyAccessTokenStore) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var accessTokenResult = await LoadOrCreateAccessToken();
        if (accessTokenResult.IsSuccess)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResult.Value.AccessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<Result<SpotifyAccessToken, string>> LoadOrCreateAccessToken()
    {
        var storedAccessTokenResult = await spotifyAccessTokenStore.LoadSpotifyAccessTokenAsync();
        if (storedAccessTokenResult.IsSuccess)
        {
            return storedAccessTokenResult.Value;
        }

        var accessTokenResult = await CreateCompletelyNewAccessToken();
        if (!accessTokenResult.IsSuccess)
        {
            return "Unable to create new access token";
        }

        return accessTokenResult.Value;
    }

    private async Task<Result<SpotifyAccessToken, string>> CreateCompletelyNewAccessToken()
    {
        var callbackCodeResult = await AuthorizeUser();
        if (!callbackCodeResult.IsSuccess)
        {
            return callbackCodeResult.Error;
        }

        var newAccessTokenResult = await RequestAccessToken(callbackCodeResult.Value);
        if (!newAccessTokenResult.IsSuccess)
        {
            return newAccessTokenResult.Error;
        }

        var accessToken = newAccessTokenResult.Value.ToSpotifyAccessToken();
        var storeResult = await spotifyAccessTokenStore.StoreSpotifyAccessTokenAsync(accessToken);
        if (!storeResult.IsSuccess)
        {
            await Console.Error.WriteLineAsync(storeResult.Error);
        }

        return accessToken;
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

        return $"Access token request failed: {apiTokenResult.Error.Message}";
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