using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Refit;

namespace SpotifyMinimalCli.SpotifyApi;

public static class ApiExceptionExtensions
{
    public static string GetSpotifyResponseErrorMessage(this ApiException apiException)
    {
        if (string.IsNullOrEmpty(apiException.Content)
            || !TryDeserializeSpotifyResponseError(apiException.Content, out var spotifyResponseError))
        {
            return apiException.Message;
        }

        if (string.IsNullOrEmpty(spotifyResponseError.Error.Message))
        {
            return apiException.Message;
        }

        return $"{spotifyResponseError.Error.Message} (Status Code {(int)apiException.StatusCode})";
    }

    private static bool TryDeserializeSpotifyResponseError(
        string content,
        [NotNullWhen(true)] out SpotifyResponseError? spotifyResponseError)
    {
        try
        {
            spotifyResponseError = JsonSerializer.Deserialize<SpotifyResponseError>(content);
            return spotifyResponseError != null;
        }
        catch (JsonException)
        {
            spotifyResponseError = null;
            return false;
        }
    }
}