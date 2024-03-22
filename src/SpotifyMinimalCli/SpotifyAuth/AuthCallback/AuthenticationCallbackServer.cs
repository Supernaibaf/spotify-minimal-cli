using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace SpotifyMinimalCli.SpotifyAuth.AuthCallback;

public class AuthenticationCallbackServer(IOptions<AuthenticationCallbackConfig> authenticationCallbackConfig)
    : IAuthenticationCallbackServer
{
    private const string CallbackPath = "callback";

    public Uri CallbackUrl => new(authenticationCallbackConfig.Value.ServerUrl, CallbackPath);

    public async Task<Result<string, string>> WaitForCallbackCode(string state)
    {
        string? code = null;
        string? error = null;

        var builder = WebApplication.CreateSlimBuilder();
        _ = builder.WebHost.UseUrls(authenticationCallbackConfig.Value.ServerUrl.ToString());

        var webApp = builder.Build();
        _ = webApp.MapGet(
            CallbackPath,
            Results<Ok<string>, BadRequest<string>> (
                [FromQuery(Name = "code")] string? callbackCode,
                [FromQuery(Name = "error")] string? callbackError,
                [FromQuery(Name = "state")] string callbackState) =>
            {
                if (callbackState != state)
                {
                    return TypedResults.BadRequest("Invalid State");
                }

                if (callbackError != null)
                {
                    error = callbackError;
                    return TypedResults.BadRequest(callbackError);
                }

                code = callbackCode;
                return TypedResults.Ok("Login successful");
            });

        webApp.Start();

        var stopwatch = Stopwatch.StartNew();

#pragma warning disable CA1508
        while (code == null && error == null &&
               stopwatch.Elapsed < authenticationCallbackConfig.Value.MaxTimeForCallback)
#pragma warning restore CA1508
        {
            await Task.Delay(authenticationCallbackConfig.Value.CallbackCheckInterval);
        }

        await webApp.StopAsync();

#pragma warning disable CA1508
        return code == null
            ? Result.Failure<string, string>(
                error ?? $"{nameof(AuthenticationCallbackConfig.MaxTimeForCallback)} has elapsed")
            : Result.Success<string, string>(code);
#pragma warning restore CA1508
    }
}