using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using CliFx;

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
