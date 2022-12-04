using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using CliFx;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("scenariokingdom", Description = "Get information on kingdom army assignments.")]
public partial class ScenarioKingdomCommand : ICommand
{
    private readonly ICurrentModService _currentModService;
    public ScenarioKingdomCommand(ICurrentModService currentModService)
    {
        _currentModService = currentModService;
    }

    [CommandParameter(0, Description = "Scenario ID.", Name = "scenarioid")]
    public ScenarioId ScenarioId { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!_currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IScenarioKingdomService>();

        var model = service.Retrieve((int)ScenarioId);

        console.Render(model, ScenarioId);

        return default;
    }
}