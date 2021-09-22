using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModelCommands
{
    [Command("scenariopokemon", Description = "Get data on a given ScenarioPokemon for a given Scenario.")]
    public class ScenarioPokemonCommand : ICommand
    {
        [CommandParameter(0, Description = "Scenario ID.", Name = "scenarioid")]
        public int ScenarioId { get; set; }

        [CommandParameter(1, Description = "ScenarioPokemon ID.", Name = "id")]
        public int ScenarioPokemonId { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var services = ConsoleAppServices.Instance;
            if (services.CurrentMod == null)
            {
                console.Output.WriteLine("No mod selected");
                return default;
            }
            var dataService = services.CoreServices.DataService(services.CurrentMod);
            var model = dataService.ScenarioPokemon.Retrieve(ScenarioId, ScenarioPokemonId);

            console.Render(model, ScenarioId, ScenarioPokemonId);

            return default;
        }
    }
}
