using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModCommands;

[Command("update mod", Description = "Update current mod with new info.")]
public class UpdateModCommand : BaseCommand
{
    public UpdateModCommand(IServiceContainer container) : base(container) { }
    public UpdateModCommand() : base() { }

    [CommandOption("name", 'n', Description = "Name of mod")]
    public string ModName { get; set; }

    [CommandOption("version", 'v', Description = "Version of mod (as defined by user)")]
    public string ModVersion { get; set; }

    [CommandOption("author", 'a', Description = "Author of mod")]
    public string ModAuthor { get; set; }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var currentModService = Container.Resolve<ICurrentModService>();
        var modService = Container.Resolve<IModService>();

        if (!currentModService.TryGetCurrentMod(console, out ModInfo currentMod))
        {
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
        modService.Update(currentMod);
        console.Output.WriteLine("Mod update successfully with new info:");
        console.Render(currentMod);
        return default;
    }
}
