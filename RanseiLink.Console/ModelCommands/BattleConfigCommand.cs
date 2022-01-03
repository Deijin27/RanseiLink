using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModelCommands;

[Command("battleconfig", Description = "Get data on a given battle config.")]
public class BattleConfigCommand : BaseCommand
{
    public BattleConfigCommand(IServiceContainer container) : base(container) { }
    public BattleConfigCommand() : base() { }

    [CommandParameter(0, Description = "BattleConfig ID.", Name = "id")]
    public BattleConfigId Id { get; set; }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var currentModService = Container.Resolve<ICurrentModService>();
        if (!currentModService.TryGetDataService(console, out IDataService dataService))
        {
            return default;
        }

        var model = dataService.BattleConfig.Retrieve(Id);

        console.Render(model, Id);

        return default;
    }
}
