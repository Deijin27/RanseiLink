using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModelCommands
{
    [Command("gimmick", Description = "Get data on a given gimmick object.")]
    public class GimmickCommand : ICommand
    {
        [CommandParameter(0, Description = "Gimmick ID.", Name = "id")]
        public GimmickId Id { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var services = ConsoleAppServices.Instance;
            if (services.CurrentMod == null)
            {
                console.Output.WriteLine("No mod selected");
                return default;
            }
            var dataService = services.CoreServices.DataService(services.CurrentMod);
            var model = dataService.Gimmick.Retrieve(Id);

            console.Render(model, Id);

            return default;
        }
    }
}
