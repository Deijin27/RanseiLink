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
    [Command("item", Description = "Get data on a given item.")]
    public class ItemCommand : ICommand
    {
        [CommandParameter(0, Description = "Item ID.", Name = "id")]
        public ItemId Id { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            IModelDataService<ItemId, IItem> service = new DataService();
            var item = service.Retrieve(Id);
            console.Render(item, Id);

            return default;
        }
    }
}
