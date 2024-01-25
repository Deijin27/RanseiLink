using RanseiLink.Console.Settings;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;

namespace RanseiLink.Console.ModCommands;

[Command("create mod", Description = "Create a new mod (and by default set the current mod to it).")]
public class CreateModCommand(IModManager modManager, ISettingService settingService) : ICommand
{
    [CommandParameter(0, Description = "Path to unchanged rom file to serve as a base.", Name = "romPath", Converter = typeof(PathConverter))]
    public string RomPath { get; set; }

    [CommandOption("name", 'n', Description = "Name of mod")]
    public string ModName { get; set; }

    [CommandOption("version", 'v', Description = "Version of mod")]
    public string ModVersion { get; set; }

    [CommandOption("author", 'a', Description = "Author of mod")]
    public string ModAuthor { get; set; }

    [CommandOption("setAsCurrent", 's', Description = "Set the current mod to the created after creation.")]
    public bool SetAsCurrent { get; set; } = true;

    public ValueTask ExecuteAsync(IConsole console)
    {
        ModInfo modInfo = modManager.Create(RomPath, new() { Name = ModName, Author = ModAuthor, Version = ModVersion });
        if (SetAsCurrent)
        {
            var mods = modManager.GetAllModInfo();
            for (int i = 0; i < mods.Count; i++)
            {
                if (mods[i].FolderPath == modInfo.FolderPath)
                {
                    settingService.Get<CurrentConsoleModSlotSetting>().Value = i;
                    settingService.Save();
                    break;
                }
            }
            
        }
        console.Output.WriteLine("Mod created successfully");
        console.Render(modInfo);

        return default;
    }
}
