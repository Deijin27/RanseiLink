using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModelCommands
{
    [Command("move", Description = "Get data on a given move.")]
    public class MoveCommand : ICommand
    {
        [CommandParameter(0, Description = "Move ID.", Name = "id")]
        public MoveId Id { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var services = ConsoleAppServices.Instance;
            if (services.CurrentMod == null)
            {
                console.Output.WriteLine("No mod selected");
                return default;
            }
            var dataService = services.CoreServices.DataService(services.CurrentMod);
            var model = dataService.Move.Retrieve(Id);

            console.Render(model, Id);

            return default;
        }
    }
}
