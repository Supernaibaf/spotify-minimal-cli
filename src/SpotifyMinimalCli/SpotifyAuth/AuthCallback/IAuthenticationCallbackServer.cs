namespace SpotifyMinimalCli.SpotifyAuth.AuthCallback;

public interface IAuthenticationCallbackServer
{
    Uri CallbackUrl { get; }

    Task<Result<string, string>> WaitForCallbackCode(string state, CancellationToken cancellationToken);
}