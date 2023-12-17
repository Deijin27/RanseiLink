using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using CliFx;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("gimmick", Description = "Get data on a given gimmick object.")]
public class GimmickCommand(ICurrentModService currentModService) : ICommand
{
    [CommandParameter(0, Description = "Gimmick ID.", Name = "id")]
    public GimmickId Id { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IGimmickService>();

        var model = service.Retrieve((int)Id);

        console.Render(model, Id);

        return default;
    }
}
