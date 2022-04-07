using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using CliFx;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("battleconfig", Description = "Get data on a given battle config.")]
public class BattleConfigCommand : ICommand
{
    private readonly ICurrentModService _currentModService;
    public BattleConfigCommand(ICurrentModService currentModService)
    {
        _currentModService = currentModService;
    }

    [CommandParameter(0, Description = "BattleConfig ID.", Name = "id")]
    public BattleConfigId Id { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!_currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IBattleConfigService>();

        var model = service.Retrieve((int)Id);

        console.Render(model, Id);

        return default;
    }
}
