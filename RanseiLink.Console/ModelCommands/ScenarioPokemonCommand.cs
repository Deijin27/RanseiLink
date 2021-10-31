using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModelCommands
{
    [Command("scenariopokemon", Description = "Get data on a given ScenarioPokemon for a given Scenario.")]
    public class ScenarioPokemonCommand : BaseCommand
    {
        public ScenarioPokemonCommand(IServiceContainer container) : base(container) { }
        public ScenarioPokemonCommand() : base() { }

        [CommandParameter(0, Description = "Scenario ID.", Name = "scenarioid")]
        public ScenarioId ScenarioId { get; set; }

        [CommandParameter(1, Description = "ScenarioPokemon ID.", Name = "id")]
        public int ScenarioPokemonId { get; set; }

        public override ValueTask ExecuteAsync(IConsole console)
        {
            var currentModService = Container.Resolve<ICurrentModService>();
            if (!currentModService.TryGetDataService(console, out IDataService dataService))
            {
                return default;
            }

            var model = dataService.ScenarioPokemon.Retrieve(ScenarioId, ScenarioPokemonId);

            console.Render(model, ScenarioId, ScenarioPokemonId);

            return default;
        }
    }
}
