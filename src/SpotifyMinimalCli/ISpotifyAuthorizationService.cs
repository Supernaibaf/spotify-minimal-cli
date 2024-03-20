namespace SpotifyMinimalCli;

public interface ISpotifyAuthorizationService
{
    Task<Result<string, string>> GetAuthorizationToken();
}