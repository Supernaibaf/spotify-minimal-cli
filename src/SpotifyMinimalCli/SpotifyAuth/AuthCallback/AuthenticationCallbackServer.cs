using System.Diagnostics;
using System.Net.Mime;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace SpotifyMinimalCli.SpotifyAuth.AuthCallback;

public class AuthenticationCallbackServer(IOptions<AuthenticationCallbackConfig> authenticationCallbackConfig)
    : IAuthenticationCallbackServer
{
    private const string CallbackPath = "callback";

    public Uri CallbackUrl => new(authenticationCallbackConfig.Value.ServerUrl, CallbackPath);

    public async Task<Result<string, string>> WaitForCallbackCode(string state, CancellationToken cancellationToken)
    {
        string? code = null;
        string? error = null;

        var builder = WebApplication.CreateSlimBuilder();
        _ = builder.WebHost.UseUrls(authenticationCallbackConfig.Value.ServerUrl.ToString());

        var webApp = builder.Build();
        _ = webApp.MapGet(
            CallbackPath,
            FileStreamHttpResult (
                [FromQuery(Name = "code")] string? callbackCode,
                [FromQuery(Name = "error")] string? callbackError,
                [FromQuery(Name = "state")] string callbackState) =>
            {
                if (callbackState != state)
                {
                    return TypedResults.File(
                        LoadEmbeddedResource("CallbackPages.FailurePage.html"),
                        MediaTypeNames.Text.Html);
                }

                if (callbackError != null)
                {
                    error = callbackError;
                    return TypedResults.File(
                        LoadEmbeddedResource("CallbackPages.FailurePage.html"),
                        MediaTypeNames.Text.Html);
                }

                code = callbackCode;
                return TypedResults.File(
                    LoadEmbeddedResource("CallbackPages.SuccessPage.html"),
                    MediaTypeNames.Text.Html);
            });

        webApp.Start();

        var stopwatch = Stopwatch.StartNew();

#pragma warning disable CA1508
        while (code == null && error == null &&
               stopwatch.Elapsed < authenticationCallbackConfig.Value.MaxTimeForCallback &&
               !cancellationToken.IsCancellationRequested)
#pragma warning restore CA1508
        {
            await Task.Delay(authenticationCallbackConfig.Value.CallbackCheckInterval, cancellationToken);
        }

        await webApp.StopAsync(cancellationToken);

#pragma warning disable CA1508
        return code == null
            ? Result.Failure<string, string>(
                error ?? $"{nameof(AuthenticationCallbackConfig.MaxTimeForCallback)} has elapsed")
            : Result.Success<string, string>(code);
#pragma warning restore CA1508
    }

    private static Stream LoadEmbeddedResource(string name)
    {
        var fileStream = typeof(AuthenticationCallbackServer).Assembly.GetManifestResourceStream(
            typeof(AuthenticationCallbackServer),
            name);

        return fileStream ?? throw new ArgumentException($"could not find resource with name \"{name}\"");
    }
}