using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModelCommands
{
    [Command("warriorskill", Description = "Get data on a given warrior skill.")]
    public class WarriorSkillCommand : BaseCommand
    {
        public WarriorSkillCommand(IServiceContainer container) : base(container) { }
        public WarriorSkillCommand() : base() { }

        [CommandParameter(0, Description = "WarriorSkill ID.", Name = "id")]
        public WarriorSkillId Id { get; set; }

        public override ValueTask ExecuteAsync(IConsole console)
        {
            var currentModService = Container.Resolve<ICurrentModService>();
            if (!currentModService.TryGetDataService(console, out IDataService dataService))
            {
                return default;
            }

            var model = dataService.WarriorSkill.Retrieve(Id);

            console.Render(model, Id);

            return default;
        }
    }
}
