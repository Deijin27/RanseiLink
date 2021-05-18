using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;
using System.Threading.Tasks;

namespace RanseiConsole.Commands
{
    [Command("kingdom", Description = "Get data on a given kingdom.")]
    public class KingdomCommand : ICommand
    {
        [CommandParameter(0, Description = "Kingdom ID.", Name = "id")]
        public KingdomId Id { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            IModelDataService<KingdomId, IKingdom> service = new DataService();
            var kingdom = service.Retrieve(Id);

            console.Render(kingdom, Id);

            return default;
        }
    }
}
