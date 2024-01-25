using RanseiLink.Core.Services;

namespace RanseiLink.Console.ModCommands;

[Command("current mod", Description = "Current Mod.")]
public class CurrentModCommand(ICurrentModService currentModService) : ICommand
{
    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentMod(out ModInfo currentMod))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        console.Render(currentMod);
        return default;
    }
}
