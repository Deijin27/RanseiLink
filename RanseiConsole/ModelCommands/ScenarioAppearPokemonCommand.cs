using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using Core.Services;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModelCommands
{
    [Command("scenarioappearpokemon", Description = "Get AppearPokemon data for a given Scenario.")]
    public class ScenarioAppearPokemonCommand : BaseCommand
    {
        public ScenarioAppearPokemonCommand(IServiceContainer container) : base(container) { }
        public ScenarioAppearPokemonCommand() : base() { }

        [CommandParameter(0, Description = "Scenario ID.", Name = "scenarioid")]
        public ScenarioId ScenarioId { get; set; }

        public override ValueTask ExecuteAsync(IConsole console)
        {
            var currentModService = Container.Resolve<ICurrentModService>();
            if (!currentModService.TryGetDataService(console, out IDataService dataService))
            {
                return default;
            }

            var model = dataService.ScenarioAppearPokemon.Retrieve(ScenarioId);

            console.Render(model, ScenarioId);

            return default;
        }
    }
}
