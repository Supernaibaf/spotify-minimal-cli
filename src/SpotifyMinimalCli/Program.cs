using System.Reflection;
using Cocona;
using SpotifyMinimalCli;
using SpotifyMinimalCli.Commands;
using SpotifyMinimalCli.SpotifyApi;
using SpotifyMinimalCli.SpotifyAuth;

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

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSpotifyAuthServices();
builder.Services.AddSpotifyApiServices();

var app = builder.Build();

app.UseCommandExceptionFilter();

app.AddQueueCommand();
app.AddNextCommand();
app.AddPreviousCommand();

await app.RunAsync();