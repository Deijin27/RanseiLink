using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using Core.Services;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModelCommands
{
    [Command("building", Description = "Get data on a given building.")]
    public class BuildingCommand : BaseCommand
    {
        public BuildingCommand(IServiceContainer container) : base(container) { }
        public BuildingCommand() : base() { }

        [CommandParameter(0, Description = "Building ID.", Name = "id")]
        public BuildingId Id { get; set; }

        public override ValueTask ExecuteAsync(IConsole console)
        {
            var currentModService = Container.Resolve<ICurrentModService>();
            if (!currentModService.TryGetDataService(console, out IDataService dataService))
            {
                return default;
            }

            var model = dataService.Building.Retrieve(Id);

            console.Render(model, Id);

            return default;
        }
    }
}
