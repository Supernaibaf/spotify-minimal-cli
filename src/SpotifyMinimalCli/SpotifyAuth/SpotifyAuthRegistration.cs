using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using Refit;
using SpotifyMinimalCli.SpotifyAuth.AccessTokenStore;
using SpotifyMinimalCli.SpotifyAuth.AccountApi;
using SpotifyMinimalCli.SpotifyAuth.AuthCallback;

namespace SpotifyMinimalCli.SpotifyAuth;

public static class SpotifyAuthRegistration
{
    public static void AddSpotifyAuthServices(this IServiceCollection services)
    {
        _ = services.AddOptions<SpotifyAccountApiConfig>().BindConfiguration(SpotifyAccountApiConfig.Key);
        _ = services.AddOptions<AuthenticationCallbackConfig>().BindConfiguration(AuthenticationCallbackConfig.Key);

        _ = services.AddTransient<ISpotifyAccessTokenStore, SpotifyAccessTokenStore>();
        _ = services.AddTransient<IAuthenticationCallbackServer, AuthenticationCallbackServer>();

        _ = services.AddTransient<SpotifyAccessTokenMessageHandler>();

        _ = services
            .AddRefitClient<ISpotifyAccountApi>()
            .ConfigureHttpClient(
                (serviceProvider, client) =>
                {
                    var config = serviceProvider.GetRequiredService<IOptions<SpotifyAccountApiConfig>>();

                    client.BaseAddress = config.Value.BaseAddress;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                        "Basic",
                        Convert.ToBase64String(
                            Encoding.UTF8.GetBytes($"{config.Value.ClientId}:{config.Value.ClientSecret}")));
                });
    }
}