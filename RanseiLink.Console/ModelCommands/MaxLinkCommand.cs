using RanseiLink.Core.Enums;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("maxlink", Description = "Get max link data for a warrior.")]
public class MaxLinkCommand(ICurrentModService currentModService) : ICommand
{
    [CommandParameter(0, Description = "Warrior ID.", Name = "id")]
    public WarriorId Id { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IMaxLinkService>();

        var model = service.Retrieve((int)Id);

        console.Render(model, Id);

        return default;
    }
}
