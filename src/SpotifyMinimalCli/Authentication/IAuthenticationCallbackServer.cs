namespace SpotifyMinimalCli.Authentication;

public interface IAuthenticationCallbackServer
{
    Uri CallbackUrl { get; }

    Task<Result<string, string>> WaitForCallbackCode(string state);
}