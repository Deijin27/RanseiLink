using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using Core.Services;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModelCommands
{
    [Command("basewarrior", Description = "Get data on a given base warrior.")]
    public class BaseWarriorCommand : BaseCommand
    {
        public BaseWarriorCommand(IServiceContainer container) : base(container) { }
        public BaseWarriorCommand() : base() { }

        [CommandParameter(0, Description = "Warrior ID.", Name = "id")]
        public WarriorId Id { get; set; }

        public override ValueTask ExecuteAsync(IConsole console)
        {
            var currentModService = Container.Resolve<ICurrentModService>();
            if (!currentModService.TryGetDataService(console, out IDataService dataService))
            {
                return default;
            }

            var model = dataService.BaseWarrior.Retrieve(Id);

            console.Render(model, Id);

            return default;
        }
    }
}
