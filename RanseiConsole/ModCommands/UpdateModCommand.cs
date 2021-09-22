using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModCommands
{
    [Command("update mod", Description = "Update current mod with new info.")]
    public class UpdateModCommand : ICommand
    {
        [CommandOption("name", 'n', Description = "Name of mod")]
        public string ModName { get; set; }

        [CommandOption("version", 'v', Description = "Version of mod (as defined by user)")]
        public string ModVersion { get; set; }

        [CommandOption("author", 'a', Description = "Author of mod")]
        public string ModAuthor { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var service = ConsoleAppServices.Instance;
            var currentMod = service.CurrentMod;
            if (currentMod == null)
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
            service.CoreServices.ModService.Update(currentMod);
            console.Output.WriteLine("Mod update successfully with new info:");
            console.Render(currentMod);
            return default;
        }
    }
}
