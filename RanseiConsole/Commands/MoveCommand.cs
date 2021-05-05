using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using Core.Models;
using Core.Services;
using System.Threading.Tasks;

namespace RanseiConsole.Commands
{
    [Command("move", Description = "Get data on a given move.")]
    public class MoveCommand : ICommand
    {
        [CommandParameter(0, Description = "Move ID.", Name = "id")]
        public MoveId Id { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            IModelDataService<MoveId, Move> service = new DataService();
            var move = service.Retrieve(Id);

            console.Render(move, Id);

            return default;
        }
    }
}
