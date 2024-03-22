using Cocona;
using Cocona.Builder;
using Cocona.Filters;

namespace SpotifyMinimalCli;

public static class CommandExceptionFilter
{
    public static ICoconaCommandsBuilder UseCommandExceptionFilter(this CoconaApp app)
    {
        return app.UseFilter(HandleCommandException);
    }

    private static async ValueTask<int> HandleCommandException(
        CoconaCommandExecutingContext context,
        CommandExecutionDelegate commandExecutionDelegate)
    {
        try
        {
            return await commandExecutionDelegate(context);
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync(e.Message);
            return -1;
        }
    }
}