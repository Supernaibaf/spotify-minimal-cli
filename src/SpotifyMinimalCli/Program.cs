using Cocona;

var builder = CoconaApp.CreateBuilder();

var app = builder.Build();
app.AddCommand(
    "queue",
    ([Argument] string[] titles) =>
    {
        foreach (var title in titles)
        {
            Console.WriteLine($"queued {title}");
        }
    });

app.Run();