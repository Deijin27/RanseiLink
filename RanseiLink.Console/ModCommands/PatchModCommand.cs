﻿using RanseiLink.Core.Services;

namespace RanseiLink.Console.ModCommands;

[Command("patch mod", Description = "Patch rom with current mod")]
public class PatchModCommand(ICurrentModService currentModService, IModPatchingService modPatcher) : ICommand
{
    [CommandParameter(0, Description = "Path to rom file.", Name = "path", Converter = typeof(PathConverter))]
    public string Path { get; set; }

    [CommandOption("includeSprites", 's', Description = "Include sprites in patch")]
    public bool IncludeSprites { get; set; } = true;

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentMod(out ModInfo currentMod))
        {
            console.WriteLine("No mod selected");
            return default;
        }

        PatchOptions patchOpt = 0;

        if (IncludeSprites)
        {
            patchOpt |= PatchOptions.IncludeSprites;
        }

        var canPatch = modPatcher.CanPatch(currentMod, Path, patchOpt);
        if (canPatch.IsFailed)
        {
            console.WriteLine(canPatch.ToString());
            return default;
        }

        modPatcher.Patch(currentMod, Path, patchOpt);
        console.WriteLine("Mod written to rom successfully. The mod that was written was the current mod:");
        console.Render(currentMod);
        return default;
    }
}
