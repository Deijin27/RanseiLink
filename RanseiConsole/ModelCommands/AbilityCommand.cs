using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using Core.Services;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModelCommands
{
    [Command("ability", Description = "Get data on a given ability.")]
    public class AbilityCommand : BaseCommand
    {
        public AbilityCommand(IServiceContainer container) : base(container) { }
        public AbilityCommand() : base() { }

        [CommandParameter(0, Description = "Ability ID.", Name = "id")]
        public AbilityId Id { get; set; }

        public override ValueTask ExecuteAsync(IConsole console)
        {
            var currentModService = Container.Resolve<ICurrentModService>();
            if (!currentModService.TryGetDataService(console, out IDataService dataService))
            {
                return default;
            }

            var model = dataService.Ability.Retrieve(Id);

            console.Render(model, Id);

            return default;
        }
    }
}
