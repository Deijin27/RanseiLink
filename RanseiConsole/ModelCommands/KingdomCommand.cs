using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModelCommands
{
    [Command("kingdom", Description = "Get data on a given kingdom.")]
    public class KingdomCommand : ICommand
    {
        [CommandParameter(0, Description = "Kingdom ID.", Name = "id")]
        public KingdomId Id { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var services = ConsoleAppServices.Instance;
            if (services.CurrentMod == null)
            {
                console.Output.WriteLine("No mod selected");
                return default;
            }
            var dataService = services.CoreServices.DataService(services.CurrentMod);
            var model = dataService.Kingdom.Retrieve(Id);

            console.Render(model, Id);

            return default;
        }
    }
}
