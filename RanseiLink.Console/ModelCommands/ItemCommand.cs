using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModelCommands
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
