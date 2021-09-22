using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Services;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModCommands
{
    [Command("import mod", Description = "Import mod (and by default sets imported mod to current)")]
    public class ImportModCommand : ICommand
    {
        [CommandParameter(0, Description = "Path to mod file.", Name = "path", Converter = typeof(PathConverter))]
        public string Path { get; set; }

        [CommandOption("setAsCurrent", 's', Description = "Set the current mod to the created after import.")]
        public bool SetAsCurrent { get; set; } = true;

        public ValueTask ExecuteAsync(IConsole console)
        {
            IConsoleAppServices services = ConsoleAppServices.Instance;
            IModService modManager = services.CoreServices.ModService;
            var info = modManager.Import(Path);
            if (SetAsCurrent)
            {
                services.CoreServices.Settings.CurrentConsoleModSlot = services.CoreServices.ModService.GetAllModInfo().Count - 1;
            }
            console.Output.WriteLine("Mod imported successfully.");
            console.Render(info);
            return default;
        }
    }
}
