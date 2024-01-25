using RanseiLink.Core.Enums;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("item", Description = "Get data on a given item.")]
public class ItemCommand(ICurrentModService currentModService) : ICommand
{
    [CommandParameter(0, Description = "Item ID.", Name = "id")]
    public ItemId Id { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IItemService>();

        var model = service.Retrieve((int)Id);

        console.Render(model, Id);

        return default;
    }
}
