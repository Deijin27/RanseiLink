using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using Core.Services;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModelCommands
{
    [Command("gimmick", Description = "Get data on a given gimmick object.")]
    public class GimmickCommand : BaseCommand
    {
        public GimmickCommand(IServiceContainer container) : base(container) { }
        public GimmickCommand() : base() { }

        [CommandParameter(0, Description = "Gimmick ID.", Name = "id")]
        public GimmickId Id { get; set; }

        public override ValueTask ExecuteAsync(IConsole console)
        {
            var currentModService = Container.Resolve<ICurrentModService>();
            if (!currentModService.TryGetDataService(console, out IDataService dataService))
            {
                return default;
            }

            var model = dataService.Gimmick.Retrieve(Id);

            console.Render(model, Id);

            return default;
        }
    }
}
