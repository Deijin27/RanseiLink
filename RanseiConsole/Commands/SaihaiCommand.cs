using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using Core.Models;
using Core.Services;
using System.Threading.Tasks;

namespace RanseiConsole.Commands
{
    [Command("saihai", Description = "Get data on a given warrior skill.")]
    public class SaihaiCommand : ICommand
    {
        [CommandParameter(0, Description = "Saihai ID.", Name = "id")]
        public SaihaiId Id { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            IDataService<SaihaiId, Saihai> service = new DataService();
            var saihai = service.Retrieve(Id);

            console.Render(saihai, Id);

            return default;
        }
    }
}
