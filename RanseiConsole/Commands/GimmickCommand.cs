using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using Core.Models;
using Core.Services;
using System.Threading.Tasks;

namespace RanseiConsole.Commands
{
    [Command("gimmick", Description = "Get data on a given gimmick object.")]
    public class GimmickCommand : ICommand
    {
        [CommandParameter(0, Description = "Gimmick ID.", Name = "id")]
        public GimmickId Id { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            IModelDataService<GimmickId, Gimmick> service = new DataService();
            var gimmick = service.Retrieve(Id);

            console.Render(gimmick, Id);

            return default;
        }
    }
}
