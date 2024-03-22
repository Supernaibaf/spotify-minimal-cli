namespace SpotifyMinimalCli.Authentication;

public class AuthenticationCallbackConfig
{
    public const string Key = "AuthenticationCallback";

    public required Uri ServerUrl { get; init; }

    public required TimeSpan MaxTimeForCallback { get; init; }

    public required TimeSpan CallbackCheckInterval { get; init; }
}