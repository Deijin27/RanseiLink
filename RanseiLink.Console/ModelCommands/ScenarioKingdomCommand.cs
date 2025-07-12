using RanseiLink.Core.Enums;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("scenariokingdom", Description = "Get information on kingdom army assignments.")]
public partial class ScenarioKingdomCommand(ICurrentModService currentModService) : ICommand
{
    [CommandParameter(0, Description = "Scenario ID.", Name = "scenarioid")]
    public ScenarioId ScenarioId { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IScenarioKingdomService>();

        var model = service.Retrieve((int)ScenarioId);

        console.Render(model, ScenarioId);

        return default;
    }
}