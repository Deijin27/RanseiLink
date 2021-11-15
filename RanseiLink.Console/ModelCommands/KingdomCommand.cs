using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModelCommands;

[Command("kingdom", Description = "Get data on a given kingdom.")]
public class KingdomCommand : BaseCommand
{
    public KingdomCommand(IServiceContainer container) : base(container) { }
    public KingdomCommand() : base() { }

    [CommandParameter(0, Description = "Kingdom ID.", Name = "id")]
    public KingdomId Id { get; set; }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var currentModService = Container.Resolve<ICurrentModService>();
        if (!currentModService.TryGetDataService(console, out IDataService dataService))
        {
            return default;
        }

        var model = dataService.Kingdom.Retrieve(Id);

        console.Render(model, Id);

        return default;
    }
}
