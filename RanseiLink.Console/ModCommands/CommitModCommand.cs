using CliFx.Attributes;
using CliFx.Infrastructure;
using System.Threading.Tasks;
using CliFx;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;

namespace RanseiLink.Console.ModCommands;

[Command("commit mod", Description = "Commit current mod to rom.")]
public class CommitModCommand : ICommand
{
    private readonly ICurrentModService _currentModService;
    private readonly IModPatchingService _modPatcher;
    public CommitModCommand(ICurrentModService currentModService, IModPatchingService modPatcher)
    {
        _currentModService = currentModService;
        _modPatcher = modPatcher;
    }

    [CommandParameter(0, Description = "Path to rom file.", Name = "path", Converter = typeof(PathConverter))]
    public string Path { get; set; }

    [CommandOption("includeSprites", 's', Description = "Include sprites in patch")]
    public bool IncludeSprites { get; set; } = true;

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!_currentModService.TryGetCurrentMod(out ModInfo currentMod))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        PatchOptions patchOpt = 0;

        if (IncludeSprites)
        {
            patchOpt |= PatchOptions.IncludeSprites;
        }

        if (!_modPatcher.CanPatch(currentMod, Path, patchOpt, out string reason))
        {
            console.Output.WriteLine(reason);
            return default;
        }

        _modPatcher.Patch(currentMod, Path, patchOpt);
        console.Output.WriteLine("Mod written to rom successfully. The mod that was written was the current mod:");
        console.Render(currentMod);
        return default;
    }
}
