using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using CliFx;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("scenariopokemon", Description = "Get data on a given ScenarioPokemon for a given Scenario.")]
public class ScenarioPokemonCommand : ICommand
{
    private readonly ICurrentModService _currentModService;
    public ScenarioPokemonCommand(ICurrentModService currentModService)
    {
        _currentModService = currentModService;
    }

    [CommandParameter(0, Description = "Scenario ID.", Name = "scenarioid")]
    public ScenarioId ScenarioId { get; set; }

    [CommandParameter(1, Description = "ScenarioPokemon ID.", Name = "id")]
    public int ScenarioPokemonId { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!_currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IScenarioPokemonService>();

        var model = service.Retrieve((int)ScenarioId).Retrieve(ScenarioPokemonId);

        console.Render(model, ScenarioId, ScenarioPokemonId);

        return default;
    }
}
