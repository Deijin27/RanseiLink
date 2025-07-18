﻿using RanseiLink.Console.Settings;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;

namespace RanseiLink.Console.ModCommands;

[Command("import mod", Description = "Import mod (and by default sets imported mod to current)")]
public class ImportModCommand(IModManager modManager, ISettingService settingService) : ICommand
{
    [CommandParameter(0, Description = "Path to mod file.", Name = "path", Converter = typeof(PathConverter))]
    public string Path { get; set; }

    [CommandOption("setAsCurrent", 's', Description = "Set the current mod to the created after import.")]
    public bool SetAsCurrent { get; set; } = true;

    public ValueTask ExecuteAsync(IConsole console)
    {
        var info = modManager.Import(Path);
        if (SetAsCurrent)
        {
            var mods = modManager.GetAllModInfo();
            for (int i = 0; i < mods.Count; i++)
            {
                if (mods[i].FolderPath == info.FolderPath)
                {
                    settingService.Get<CurrentConsoleModSlotSetting>().Value = i;
                    settingService.Save();
                    break;
                }
            }
        }
        console.WriteLine("Mod imported successfully.");
        console.Render(info);
        return default;
    }
}
