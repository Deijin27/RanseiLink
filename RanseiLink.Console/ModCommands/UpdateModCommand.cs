using RanseiLink.Core.Services;

namespace RanseiLink.Console.ModCommands;

[Command("update mod", Description = "Update current mod with new info.")]
public class UpdateModCommand(ICurrentModService currentModService, IModManager modManager) : ICommand
{
    [CommandOption("name", 'n', Description = "Name of mod")]
    public string ModName { get; set; }

    [CommandOption("version", 'v', Description = "Version of mod (as defined by user)")]
    public string ModVersion { get; set; }

    [CommandOption("author", 'a', Description = "Author of mod")]
    public string ModAuthor { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentMod(out ModInfo currentMod))
        {
            console.WriteLine("No mod selected");
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
        modManager.Update(currentMod);
        console.WriteLine("Mod update successfully with new info:");
        console.Render(currentMod);
        return default;
    }
}
