using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using CliFx;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("episode", Description = "Get data on a given episode.")]
public class EpisodeCommand(ICurrentModService currentModService) : ICommand
{
    [CommandParameter(0, Description = "Episode ID.", Name = "id")]
    public EpisodeId Id { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IEpisodeService>();

        var model = service.Retrieve((int)Id);

        console.Render(model, Id);

        return default;
    }
}
