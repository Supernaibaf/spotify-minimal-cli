namespace SpotifyMinimalCli.SpotifyApi;

public interface ISpotifyAuthorizationService
{
    Task<Result<string, string>> GetAuthorizationToken();
}