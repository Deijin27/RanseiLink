using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using CliFx;

namespace RanseiLink.Console.ModCommands;

[Command("delete mod", Description = "Delete current mod.")]
public class DeleteModCommand : ICommand
{
    private readonly ICurrentModService _currentModService;
    private readonly IModManager _modManager;
    public DeleteModCommand(ICurrentModService currentModService, IModManager modManager)
    {
        _currentModService = currentModService;
        _modManager = modManager;
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!_currentModService.TryGetCurrentMod(out ModInfo currentMod))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        _modManager.Delete(currentMod);
        console.Output.WriteLine("Current mod deleted. Info of deleted mod:");
        console.Render(currentMod);
        return default;
    }
}
