using RanseiLink.Core.Enums;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("kingdom", Description = "Get data on a given kingdom.")]
public class KingdomCommand(ICurrentModService currentModService) : ICommand
{
    [CommandParameter(0, Description = "Kingdom ID.", Name = "id")]
    public KingdomId Id { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IKingdomService>();

        var model = service.Retrieve((int)Id);

        console.Render(model, Id);

        return default;
    }
}
