using System.Reflection;
using Cocona;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using SpotifyMinimalCli;
using SpotifyMinimalCli.Commands;

var executableDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
if (executableDirectory == null)
{
    await Console.Error.WriteLineAsync("Unable to get executable directory name");
    return;
}

var builder = CoconaApp.CreateBuilder();

builder.Configuration
    .SetBasePath(executableDirectory)
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

app.AddQueueCommand();

await app.RunAsync();