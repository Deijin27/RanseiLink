﻿using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModCommands;

[Command("upgrade mods", Description = "Upgrade all outdated mods to latest version.")]
public class UpgradeModsCommand : BaseCommand
{
    public UpgradeModsCommand(IServiceContainer container) : base(container) { }
    public UpgradeModsCommand() : base() { }

    [CommandParameter(0, Description = "Path to unchanged rom file to serve as a source for upgrade data.", Name = "romPath", Converter = typeof(PathConverter))]
    public string RomPath { get; set; }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var modService = Container.Resolve<IModService>();

        var mods = modService.GetModInfoPreviousVersions();
        if (mods.Count == 0)
        {
            console.Output.WriteLine("All mods are already up to date");
        }
        else
        {
            modService.UpgradeModsToLatestVersion(mods, RomPath);
        }

        console.Output.WriteLine("Upgrading Complete!\n\nMods that were upgraded:");
        foreach (var mod in mods)
        {
            console.Render(mod);
        }
        return default;
    }
}