using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using Core.Services;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModelCommands
{
    [Command("item", Description = "Get data on a given item.")]
    public class ItemCommand : BaseCommand
    {
        public ItemCommand(IServiceContainer container) : base(container) { }
        public ItemCommand() : base() { }

        [CommandParameter(0, Description = "Item ID.", Name = "id")]
        public ItemId Id { get; set; }

        public override ValueTask ExecuteAsync(IConsole console)
        {
            var currentModService = Container.Resolve<ICurrentModService>();
            if (!currentModService.TryGetDataService(console, out IDataService dataService))
            {
                return default;
            }

            var model = dataService.Item.Retrieve(Id);

            console.Render(model, Id);

            return default;
        }
    }
}
