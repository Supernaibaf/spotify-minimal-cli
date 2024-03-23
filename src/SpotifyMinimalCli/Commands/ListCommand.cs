using Cocona;
using SpotifyMinimalCli.SpotifyApi;
using SpotifyMinimalCli.SpotifyApi.Player;

namespace SpotifyMinimalCli.Commands;

public static class ListCommand
{
    public static void AddListCommand(this CoconaApp app)
    {
        _ = app.AddCommand("list", List)
            .WithDescription("Lists the current queue");
    }

    private static async Task<int> List(ISpotifyApi spotifyApi, CoconaAppContext context)
    {
        var queueResponse = await spotifyApi.GetUserQueueAsync(context.CancellationToken);
        if (!queueResponse.IsSuccessStatusCode)
        {
            await Console.Error.WriteLineAsync($"Unable to get current queue: {queueResponse.Error}");
            return -1;
        }

        if (queueResponse.Content.CurrentlyPlaying != null)
        {
            Console.WriteLine(
                $"Currently playing: {GetQueueItemDisplayValue(queueResponse.Content.CurrentlyPlaying)}");
            Console.WriteLine();
        }

        if (queueResponse.Content.Queue.Count != 0)
        {
            Console.WriteLine("Queue:");
            foreach (var queueItem in queueResponse.Content.Queue)
            {
                Console.WriteLine($"- {GetQueueItemDisplayValue(queueItem)}");
            }
        }
        else
        {
            Console.WriteLine("Queue is empty");
        }

        return 0;
    }

    private static string GetQueueItemDisplayValue(QueueItem item)
    {
        return item.Match(
            track => track.DisplayValue,
            episode => episode.DisplayValue
        );
    }
}