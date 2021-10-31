using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModelCommands
{
    [Command("scenariokingdom", Description = "Get evolution table data.")]
    public class ScenarioKingdomCommand : BaseCommand
    {
        public ScenarioKingdomCommand(IServiceContainer container) : base(container) { }
        public ScenarioKingdomCommand() : base() { }

        [CommandParameter(0, Description = "Scenario ID.", Name = "scenarioid")]
        public ScenarioId ScenarioId { get; set; }

        public override ValueTask ExecuteAsync(IConsole console)
        {
            var currentModService = Container.Resolve<ICurrentModService>();
            if (!currentModService.TryGetDataService(console, out IDataService dataService))
            {
                return default;
            }

            var model = dataService.ScenarioKingdom.Retrieve(ScenarioId);

            console.Render(model, ScenarioId);

            return default;
        }
    }
}
