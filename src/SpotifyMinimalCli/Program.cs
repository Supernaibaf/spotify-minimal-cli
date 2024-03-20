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
builder.Services.AddOptions<SpotifyApiConfig>().BindConfiguration(SpotifyApiConfig.Key);

builder.Services.AddTransient<ISpotifyAuthorizationService, SpotifyAuthorizationService>();

builder.Services
    .AddRefitClient<ISpotifyAccountApi>()
    .ConfigureHttpClient(
        (services, client) =>
        {
            var config = services.GetRequiredService<IOptions<SpotifyAccountApiConfig>>();
            client.BaseAddress = new Uri(config.Value.BaseAddress);
        });

builder.Services
    .AddRefitClient<ISpotifyApi>()
    .ConfigureHttpClient(
        (services, client) =>
        {
            var config = services.GetRequiredService<IOptions<SpotifyApiConfig>>();
            client.BaseAddress = new Uri(config.Value.BaseAddress);
        });

var app = builder.Build();
app.AddCommand(
    "queue",
    async (
        [Argument] string[] trackNames,
        ISpotifyAuthorizationService authorizationService,
        ISpotifyApi spotifyApi) =>
    {
        var tokenResult = await authorizationService.GetAuthorizationToken();

        if (!tokenResult.IsSuccess)
        {
            await Console.Error.WriteLineAsync(tokenResult.Error);
            return;
        }

        foreach (var trackName in trackNames)
        {
            var searchResponse = await spotifyApi.SearchAsync(
                new SearchRequest
                {
                    Query = trackName,
                    Type = SearchType.Track,
                    Limit = 1,
                },
                tokenResult.Value);

            if (!searchResponse.IsSuccessStatusCode)
            {
                await Console.Error.WriteLineAsync(searchResponse.Error.Message);
                continue;
            }

            if (searchResponse.Content.Tracks == null || searchResponse.Content.Tracks.Items.Count == 0)
            {
                await Console.Error.WriteLineAsync($"Unable to find track with name {trackName}");
                continue;
            }

            Console.WriteLine($"queued {searchResponse.Content.Tracks.Items.First().Name}");
        }
    });

app.Run();