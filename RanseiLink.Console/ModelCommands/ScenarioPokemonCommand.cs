using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("scenariopokemon", Description = "Get data on a given ScenarioPokemon for a given Scenario.")]
public class ScenarioPokemonCommand(ICurrentModService currentModService) : ICommand
{
    [CommandParameter(0, Description = "Scenario ID.", Name = "scenarioid")]
    public ScenarioId ScenarioId { get; set; }

    [CommandParameter(1, Description = "ScenarioPokemon ID.", Name = "id")]
    public int ScenarioPokemonId { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IScenarioPokemonService>();

        var model = service.Retrieve((int)ScenarioId).Retrieve(ScenarioPokemonId);

        console.Render(model, $"Scenario = {ScenarioId}, Entry = {ScenarioPokemonId}");

        return default;
    }
}
