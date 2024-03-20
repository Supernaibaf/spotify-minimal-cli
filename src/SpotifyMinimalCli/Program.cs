using Cocona;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using SpotifyMinimalCli;
using SpotifyMinimalCli.SpotifyApiDtos;

var builder = CoconaApp.CreateBuilder();

builder.Configuration
    .AddJsonFile("appsettings.json", false)
    .AddUserSecrets<Program>();

builder.Services.AddOptions<SpotifyAccountApiConfig>().BindConfiguration(SpotifyAccountApiConfig.Key);

builder.Services
    .AddRefitClient<ISpotifyAccountApi>()
    .ConfigureHttpClient(
        (services, client) =>
        {
            var config = services.GetRequiredService<IOptions<SpotifyAccountApiConfig>>();
            client.BaseAddress = new Uri(config.Value.BaseAddress);
        });

var app = builder.Build();
app.AddCommand(
    "queue",
    async ([Argument] string[] titles, ISpotifyAccountApi spotifyAccountApi, IOptions<SpotifyAccountApiConfig> accountApiConfig) =>
    {
        var tokenResponse = await spotifyAccountApi.RequestApiTokenAsync(
            new TokenRequest
            {
                GrantType = accountApiConfig.Value.GrantType,
                ClientId = accountApiConfig.Value.ClientId,
                ClientSecret = accountApiConfig.Value.ClientSecret,
            });

        if (tokenResponse.IsSuccessStatusCode)
        {
            Console.WriteLine(tokenResponse.Content.AccessToken);
        }
        else
        {
            Console.WriteLine(tokenResponse.Error);
        }

        foreach (var title in titles)
        {
            Console.WriteLine($"queued {title}");
        }
    });

app.Run();