using System.Diagnostics;
using System.Net;
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
    ISpotifyAccessTokenStore spotifyAccessTokenStore,
    TimeProvider timeProvider) : DelegatingHandler
{
    private static readonly TimeSpan MaxTimeUntilAccessTokenExpires = TimeSpan.FromMinutes(10);

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var accessTokenResult = await LoadOrCreateAccessToken(cancellationToken);
        if (accessTokenResult.IsSuccess)
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessTokenResult.Value.AccessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        // when unauthorized, get new token and try again
        var newAccessTokenResult = await CreateCompletelyNewAccessToken(cancellationToken);
        if (newAccessTokenResult.IsSuccess)
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", newAccessTokenResult.Value.AccessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<Result<SpotifyAccessToken, string>> LoadOrCreateAccessToken(CancellationToken cancellationToken)
    {
        var storedAccessTokenResult = await spotifyAccessTokenStore.LoadSpotifyAccessTokenAsync();
        if (storedAccessTokenResult.IsSuccess)
        {
            var accessToken = storedAccessTokenResult.Value;
            if (accessToken.ExpiresAt - MaxTimeUntilAccessTokenExpires > timeProvider.GetUtcNow())
            {
                return accessToken;
            }

            var refreshedAccessToken = await RefreshAccessToken(accessToken);
            if (refreshedAccessToken.IsSuccess)
            {
                return refreshedAccessToken.Value;
            }
        }

        var accessTokenResult = await CreateCompletelyNewAccessToken(cancellationToken);
        if (!accessTokenResult.IsSuccess)
        {
            return "Unable to create new access token";
        }

        return accessTokenResult.Value;
    }

    private async Task<Result<SpotifyAccessToken, string>> RefreshAccessToken(SpotifyAccessToken accessToken)
    {
        var refreshedAccessTokenResponse = await spotifyAccountApi.RefreshAccessTokenAsync(
            new RefreshAccessTokenRequest
            {
                GrantType = "refresh_token",
                RefreshToken = accessToken.RefreshToken,
            });

        if (!refreshedAccessTokenResponse.IsSuccessStatusCode)
        {
            return $"Access token request failed: {refreshedAccessTokenResponse.Error.Message}";
        }

        var refreshedAccessToken = refreshedAccessTokenResponse.Content.ToSpotifyAccessToken(accessToken, timeProvider);
        var storeResult = await spotifyAccessTokenStore.StoreSpotifyAccessTokenAsync(refreshedAccessToken);
        if (!storeResult.IsSuccess)
        {
            await Console.Error.WriteLineAsync(storeResult.Error);
        }

        return refreshedAccessToken;
    }

    private async Task<Result<SpotifyAccessToken, string>> CreateCompletelyNewAccessToken(
        CancellationToken cancellationToken)
    {
        var callbackCodeResult = await AuthorizeUser(cancellationToken);
        if (!callbackCodeResult.IsSuccess)
        {
            return callbackCodeResult.Error;
        }

        var newAccessTokenResult = await RequestAccessToken(callbackCodeResult.Value);
        if (!newAccessTokenResult.IsSuccess)
        {
            return newAccessTokenResult.Error;
        }

        var accessToken = newAccessTokenResult.Value.ToSpotifyAccessToken(timeProvider);
        var storeResult = await spotifyAccessTokenStore.StoreSpotifyAccessTokenAsync(accessToken);
        if (!storeResult.IsSuccess)
        {
            await Console.Error.WriteLineAsync(storeResult.Error);
        }

        return accessToken;
    }

    private async Task<Result<string, string>> AuthorizeUser(CancellationToken cancellationToken)
    {
        var state = Guid.NewGuid().ToString();
        var authenticationUrl = CreateAuthorizeUrl(state);

        Console.WriteLine($"For Login open the following Url in a browser: {authenticationUrl}");

        await OpenBrowser(authenticationUrl);

        var callbackCodeResult = await authenticationCallbackServer.WaitForCallbackCode(state, cancellationToken);

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