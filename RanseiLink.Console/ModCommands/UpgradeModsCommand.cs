using RanseiLink.Core.Services;

namespace RanseiLink.Console.ModCommands;

[Command("upgrade mods", Description = "Upgrade all outdated mods to latest version.")]
public class UpgradeModsCommand(IModManager modManager) : ICommand
{
    [CommandParameter(0, Description = "Path to unchanged rom file to serve as a source for upgrade data.", Name = "romPath", Converter = typeof(PathConverter))]
    public string RomPath { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        var mods = modManager.GetModInfoPreviousVersions();
        if (mods.Count == 0)
        {
            console.Output.WriteLine("All mods are already up to date");
        }
        else
        {
            modManager.UpgradeModsToLatestVersion(mods, RomPath);
        }

        console.Output.WriteLine("Upgrading Complete!\n\nMods that were upgraded:");
        foreach (var mod in mods)
        {
            console.Render(mod);
        }
        return default;
    }
}
