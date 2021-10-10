using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Services;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModelCommands
{
    [Command("warriornametable", Description = "Get warrior name table data.")]
    public class WarriorNameTableCommand : BaseCommand
    {
        public WarriorNameTableCommand(IServiceContainer container) : base(container) { }
        public WarriorNameTableCommand() : base() { }

        public override ValueTask ExecuteAsync(IConsole console)
        {
            var currentModService = Container.Resolve<ICurrentModService>();
            if (!currentModService.TryGetDataService(console, out IDataService dataService))
            {
                return default;
            }

            var model = dataService.BaseWarrior.RetrieveNameTable();

            console.Render(model);

            return default;
        }
    }
}
