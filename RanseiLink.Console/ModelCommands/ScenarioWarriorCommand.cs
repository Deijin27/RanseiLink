using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModelCommands
{
    [Command("scenariowarrior", Description = "Get data on a given ScenarioPokemon for a given Scenario.")]
    public class ScenarioWarriorCommand : BaseCommand
    {
        public ScenarioWarriorCommand(IServiceContainer container) : base(container) { }
        public ScenarioWarriorCommand() : base() { }

        [CommandParameter(0, Description = "Scenario ID.", Name = "scenarioid")]
        public ScenarioId ScenarioId { get; set; }

        [CommandParameter(1, Description = "ScenarioWarrior ID.", Name = "id")]
        public int ScenarioWarriorId { get; set; }

        public override ValueTask ExecuteAsync(IConsole console)
        {
            var currentModService = Container.Resolve<ICurrentModService>();
            if (!currentModService.TryGetDataService(console, out IDataService dataService))
            {
                return default;
            }

            var model = dataService.ScenarioWarrior.Retrieve(ScenarioId, ScenarioWarriorId);

            console.Render(model, ScenarioId, ScenarioWarriorId);

            return default;
        }
    }
}
