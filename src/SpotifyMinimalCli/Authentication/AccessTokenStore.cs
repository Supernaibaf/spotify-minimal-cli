using System.Text.Json;

namespace SpotifyMinimalCli.Authentication;

public class AccessTokenStore : IAccessTokenStore
{
    private static readonly string ApplicationDataDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "spotify-minimal-cli");

    private static readonly string SpotifyAccessTokenFile = Path.Combine(ApplicationDataDirectory, "access-token.json");

    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    public async Task<Result<SpotifyAccessToken, string>> LoadSpotifyAccessTokenAsync()
    {
        if (!File.Exists(SpotifyAccessTokenFile))
        {
            return "Unable to load access token: File does not exist";
        }

        try
        {
            await using var inputFile = File.OpenRead(SpotifyAccessTokenFile);
            var spotifyAccessToken = await JsonSerializer.DeserializeAsync<SpotifyAccessToken>(inputFile);
            if (spotifyAccessToken == null)
            {
                return "Unable to load access token: Deserialization returned null";
            }

            return spotifyAccessToken;
        }
        catch (Exception e)
        {
            return $"Unable to load access token: {e.Message}";
        }
    }

    public async Task<VoidResult<string>> StoreSpotifyAccessTokenAsync(SpotifyAccessToken spotifyAccessToken)
    {
        try
        {
            _ = Directory.CreateDirectory(ApplicationDataDirectory);
            await using var outputFile = File.Open(SpotifyAccessTokenFile, FileMode.Create, FileAccess.Write);
            await JsonSerializer.SerializeAsync(outputFile, spotifyAccessToken, SerializerOptions);
            return VoidResult.Success<string>();
        }
        catch (Exception e)
        {
            return $"Unable to store access token: {e.Message}";
        }
    }
}