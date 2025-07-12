using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("warriornametable", Description = "Get warrior name table data.")]
public class WarriorNameTableCommand(ICurrentModService currentModService) : ICommand
{
    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IBaseWarriorService>();

        var model = service.NameTable;

        console.Render(model);

        return default;
    }
}
