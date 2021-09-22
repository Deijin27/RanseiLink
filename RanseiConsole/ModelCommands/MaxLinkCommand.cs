using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModelCommands
{
    [Command("maxlink", Description = "Get max link data for a warrior.")]
    public class MaxLinkCommand : ICommand
    {
        [CommandParameter(0, Description = "Warrior ID.", Name = "id")]
        public WarriorId Id { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var services = ConsoleAppServices.Instance;
            if (services.CurrentMod == null)
            {
                console.Output.WriteLine("No mod selected");
                return default;
            }
            var dataService = services.CoreServices.DataService(services.CurrentMod);
            var model = dataService.MaxLink.Retrieve(Id);

            console.Render(model, Id);

            return default;
        }
    }
}
