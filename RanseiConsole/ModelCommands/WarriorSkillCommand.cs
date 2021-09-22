using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModelCommands
{
    [Command("warriorskill", Description = "Get data on a given warrior skill.")]
    public class WarriorSkillCommand : ICommand
    {
        [CommandParameter(0, Description = "WarriorSkill ID.", Name = "id")]
        public WarriorSkillId Id { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var services = ConsoleAppServices.Instance;
            if (services.CurrentMod == null)
            {
                console.Output.WriteLine("No mod selected");
                return default;
            }
            var dataService = services.CoreServices.DataService(services.CurrentMod);
            var model = dataService.WarriorSkill.Retrieve(Id);

            console.Render(model, Id);

            return default;
        }
    }
}
