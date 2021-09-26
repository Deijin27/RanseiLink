using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModelCommands
{
    [Command("scenariowarrior", Description = "Get data on a given ScenarioPokemon for a given Scenario.")]
    public class ScenarioWarriorCommand : ICommand
    {
        [CommandParameter(0, Description = "Scenario ID.", Name = "scenarioid")]
        public ScenarioId ScenarioId { get; set; }

        [CommandParameter(1, Description = "ScenarioWarrior ID.", Name = "id")]
        public int ScenarioWarriorId { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var services = ConsoleAppServices.Instance;
            if (services.CurrentMod == null)
            {
                console.Output.WriteLine("No mod selected");
                return default;
            }
            var dataService = services.CoreServices.DataService(services.CurrentMod);
            var model = dataService.ScenarioPokemon.Retrieve(ScenarioId, ScenarioWarriorId);

            console.Render(model, ScenarioId, ScenarioWarriorId);

            return default;
        }
    }
}
