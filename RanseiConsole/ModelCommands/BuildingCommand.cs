using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModelCommands
{
    [Command("building", Description = "Get data on a given building.")]
    public class BuildingCommand : ICommand
    {
        [CommandParameter(0, Description = "Building ID.", Name = "id")]
        public BuildingId Id { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var services = ConsoleAppServices.Instance;
            if (services.CurrentMod == null)
            {
                console.Output.WriteLine("No mod selected");
                return default;
            }
            var dataService = services.CoreServices.DataService(services.CurrentMod);
            var model = dataService.Building.Retrieve(Id);

            console.Render(model, Id);

            return default;
        }
    }
}
