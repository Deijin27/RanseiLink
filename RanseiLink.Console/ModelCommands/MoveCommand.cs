using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModelCommands
{
    [Command("move", Description = "Get data on a given move.")]
    public class MoveCommand : BaseCommand
    {
        public MoveCommand(IServiceContainer container) : base(container) { }
        public MoveCommand() : base() { }

        [CommandParameter(0, Description = "Move ID.", Name = "id")]
        public MoveId Id { get; set; }

        public override ValueTask ExecuteAsync(IConsole console)
        {
            var currentModService = Container.Resolve<ICurrentModService>();
            if (!currentModService.TryGetDataService(console, out IDataService dataService))
            {
                return default;
            }

            var model = dataService.Move.Retrieve(Id);

            console.Render(model, Id);

            return default;
        }
    }
}
