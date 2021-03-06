using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using CliFx;

namespace RanseiLink.Console.ModCommands;

[Command("update mod", Description = "Update current mod with new info.")]
public class UpdateModCommand : ICommand
{
    private readonly ICurrentModService _currentModService;
    private readonly IModManager _modManager;
    public UpdateModCommand(ICurrentModService currentModService, IModManager modManager)
    {
        _currentModService = currentModService;
        _modManager = modManager;
    }

    [CommandOption("name", 'n', Description = "Name of mod")]
    public string ModName { get; set; }

    [CommandOption("version", 'v', Description = "Version of mod (as defined by user)")]
    public string ModVersion { get; set; }

    [CommandOption("author", 'a', Description = "Author of mod")]
    public string ModAuthor { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!_currentModService.TryGetCurrentMod(out ModInfo currentMod))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }
        if (ModName != null)
        {
            currentMod.Name = ModName;
        }
        if (ModVersion != null)
        {
            currentMod.Version = ModVersion;
        }
        if (ModAuthor != null)
        {
            currentMod.Author = ModAuthor;
        }
        _modManager.Update(currentMod);
        console.Output.WriteLine("Mod update successfully with new info:");
        console.Render(currentMod);
        return default;
    }
}
