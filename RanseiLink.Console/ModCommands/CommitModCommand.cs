using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModCommands;

[Command("commit mod", Description = "Commit current mod to rom.")]
public class CommitModCommand : BaseCommand
{
    public CommitModCommand(IServiceContainer container) : base(container) { }
    public CommitModCommand() : base() { }

    [CommandParameter(0, Description = "Path to rom file.", Name = "path", Converter = typeof(PathConverter))]
    public string Path { get; set; }

    [CommandOption("includeSprites", 's', Description = "Include sprites in patch")]
    public bool IncludeSprites { get; set; } = true;

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var currentModService = Container.Resolve<ICurrentModService>();
        var modService = Container.Resolve<IModService>();

        if (!currentModService.TryGetCurrentMod(console, out ModInfo currentMod))
        {
            return default;
        }

        PatchOptions patchOpt = 0;

        if (IncludeSprites)
        {
            patchOpt |= PatchOptions.IncludeSprites;
        }

        modService.Commit(currentMod, Path, patchOpt);
        console.Output.WriteLine("Mod written to rom successfully. The mod that was written was the current mod:");
        console.Render(currentMod);
        return default;
    }
}
