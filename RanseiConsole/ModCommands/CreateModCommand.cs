using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Services;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModCommands
{
    [Command("create mod", Description = "Create a new mod (and by default set the current mod to it).")]
    public class CreateModCommand : ICommand
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
            ICoreAppServices services = ConsoleAppServices.Instance.CoreServices;
            ModInfo modInfo = services.ModService.Create(RomPath, ModName, ModVersion, ModAuthor);
            if (SetAsCurrent)
            {
                services.Settings.CurrentConsoleModSlot = services.ModService.GetAllModInfo().Count - 1;
            }
            console.Output.WriteLine("Mod created successfully");
            console.Render(modInfo);

            return default;
        }
    }
}
