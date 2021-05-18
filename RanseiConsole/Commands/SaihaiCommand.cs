using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using Core.Models;
using Core.Models.Interfaces;
using Core.Services;
using System.Threading.Tasks;

namespace RanseiConsole.Commands
{
    [Command("saihai", Description = "Get data on a given warrior skill.")]
    public class WarriorSkillCommand : ICommand
    {
        [CommandParameter(0, Description = "WarriorSkill ID.", Name = "id")]
        public WarriorSkillId Id { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            IModelDataService<WarriorSkillId, IWarriorSkill> service = new DataService();
            var saihai = service.Retrieve(Id);

            console.Render(saihai, Id);

            return default;
        }
    }
}
