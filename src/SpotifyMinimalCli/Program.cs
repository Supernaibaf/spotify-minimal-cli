using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using Cocona;
using Microsoft.Extensions.Options;
using Refit;
using SpotifyMinimalCli.AuthenticationCallback;
using SpotifyMinimalCli.Commands;
using SpotifyMinimalCli.SpotifyApi;

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

builder.Services.AddOptions<AuthenticationCallbackConfig>().BindConfiguration(AuthenticationCallbackConfig.Key);
builder.Services.AddOptions<SpotifyAccountApiConfig>().BindConfiguration(SpotifyAccountApiConfig.Key);
builder.Services.AddOptions<SpotifyApiConfig>().BindConfiguration(SpotifyApiConfig.Key);

builder.Services.AddTransient<IAuthenticationCallbackServer, AuthenticationCallbackServer>();
builder.Services.AddTransient<ISpotifyAuthorizationService, SpotifyAuthorizationService>();

builder.Services
    .AddRefitClient<ISpotifyAccountApi>()
    .ConfigureHttpClient(
        (services, client) =>
        {
            var config = services.GetRequiredService<IOptions<SpotifyAccountApiConfig>>();

            client.BaseAddress = config.Value.BaseAddress;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{config.Value.ClientId}:{config.Value.ClientSecret}")));
        });

builder.Services
    .AddRefitClient<ISpotifyApi>()
    .ConfigureHttpClient(
        (services, client) =>
        {
            var config = services.GetRequiredService<IOptions<SpotifyApiConfig>>();
            client.BaseAddress = config.Value.BaseAddress;
        });

var app = builder.Build();

app.AddQueueCommand();
app.AddNextCommand();
app.AddPreviousCommand();

await app.RunAsync();