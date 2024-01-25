using RanseiLink.Core.Enums;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("building", Description = "Get data on a given building.")]
public class BuildingCommand(ICurrentModService currentModService) : ICommand
{
    [CommandParameter(0, Description = "Building ID.", Name = "id")]
    public BuildingId Id { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IBuildingService>();

        var model = service.Retrieve((int)Id);

        console.Render(model, Id);

        return default;
    }
}
