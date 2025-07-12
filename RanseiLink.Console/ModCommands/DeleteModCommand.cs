using RanseiLink.Core.Services;

namespace RanseiLink.Console.ModCommands;

[Command("delete mod", Description = "Delete current mod.")]
public class DeleteModCommand(ICurrentModService currentModService, IModManager modManager) : ICommand
{
    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentMod(out ModInfo currentMod))
        {
            console.WriteLine("No mod selected");
            return default;
        }

        modManager.Delete(currentMod);
        console.WriteLine("Current mod deleted. Info of deleted mod:");
        console.Render(currentMod);
        return default;
    }
}
