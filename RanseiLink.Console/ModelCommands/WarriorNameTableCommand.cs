using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using CliFx;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("warriornametable", Description = "Get warrior name table data.")]
public class WarriorNameTableCommand : ICommand
{
    private readonly ICurrentModService _currentModService;
    public WarriorNameTableCommand(ICurrentModService currentModService)
    {
        _currentModService = currentModService;
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!_currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IBaseWarriorService>();

        var model = service.NameTable;

        console.Render(model);

        return default;
    }
}
