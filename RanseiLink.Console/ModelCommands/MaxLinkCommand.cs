using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModelCommands
{
    [Command("maxlink", Description = "Get max link data for a warrior.")]
    public class MaxLinkCommand : BaseCommand
    {
        public MaxLinkCommand(IServiceContainer container) : base(container) { }
        public MaxLinkCommand() : base() { }

        [CommandParameter(0, Description = "Warrior ID.", Name = "id")]
        public WarriorId Id { get; set; }

        public override ValueTask ExecuteAsync(IConsole console)
        {
            var currentModService = Container.Resolve<ICurrentModService>();
            if (!currentModService.TryGetDataService(console, out IDataService dataService))
            {
                return default;
            }

            var model = dataService.MaxLink.Retrieve(Id);

            console.Render(model, Id);

            return default;
        }
    }
}
