using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using CliFx;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("move", Description = "Get data on a given move.")]
public class MoveCommand : ICommand
{
    private readonly ICurrentModService _currentModService;
    public MoveCommand(ICurrentModService currentModService)
    {
        _currentModService = currentModService;
    }

    [CommandParameter(0, Description = "Move ID.", Name = "id")]
    public MoveId Id { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!_currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IMoveService>();

        var model = service.Retrieve((int)Id);

        console.Render(model, Id);

        return default;
    }
}
