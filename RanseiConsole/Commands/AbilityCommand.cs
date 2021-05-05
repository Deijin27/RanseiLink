using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using Core.Models;
using Core.Services;
using System.Threading.Tasks;

namespace RanseiConsole.Commands
{
    [Command("ability", Description = "Get data on a given ability.")]
    public class AbilityCommand : ICommand
    {
        [CommandParameter(0, Description = "Ability ID.", Name = "id")]
        public AbilityId Id { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            IModelDataService<AbilityId, Ability> service = new DataService();
            var ability = service.Retrieve(Id);

            console.Render(ability, Id);

            return default;
        }
    }
}
