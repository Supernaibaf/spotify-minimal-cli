using Microsoft.Extensions.Options;
using Refit;
using SpotifyMinimalCli.SpotifyAuth;

namespace SpotifyMinimalCli.SpotifyApi;

public static class SpotifyApiRegistration
{
    public static void AddSpotifyApiServices(this IServiceCollection services)
    {
        _ = services.AddOptions<SpotifyApiConfig>().BindConfiguration(SpotifyApiConfig.Key);

        _ = services
            .AddRefitClient<ISpotifyApi>()
            .ConfigureHttpClient(
                (serviceProvider, client) =>
                {
                    var config = serviceProvider.GetRequiredService<IOptions<SpotifyApiConfig>>();
                    client.BaseAddress = config.Value.BaseAddress;
                })
            .AddHttpMessageHandler<SpotifyAccessTokenMessageHandler>();
    }
}