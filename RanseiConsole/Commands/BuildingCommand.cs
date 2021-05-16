using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;
using System.Threading.Tasks;

namespace RanseiConsole.Commands
{
    [Command("building", Description = "Get data on a given building.")]
    public class BuildingCommand : ICommand
    {
        [CommandParameter(0, Description = "Building ID.", Name = "id")]
        public BuildingId Id { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            IModelDataService<BuildingId, IBuilding> service = new DataService();
            var building = service.Retrieve(Id);

            console.Render(building, Id);

            return default;
        }
    }
}
