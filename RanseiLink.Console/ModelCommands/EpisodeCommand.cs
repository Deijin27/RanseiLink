using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using CliFx;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("episode", Description = "Get data on a given episode.")]
public class EpisodeCommand : ICommand
{
    private readonly ICurrentModService _currentModService;
    public EpisodeCommand(ICurrentModService currentModService)
    {
        _currentModService = currentModService;
    }

    [CommandParameter(0, Description = "Episode ID.", Name = "id")]
    public EpisodeId Id { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!_currentModService.TryGetCurrentModServiceGetter(out var services))
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
