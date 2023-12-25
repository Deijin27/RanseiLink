using System.Collections.Concurrent;

namespace RanseiLink.Core.Services.ModPatchBuilders;

[PatchBuilder]
public class MapModelPatchBuilder(IOverrideDataProvider overrideProvider) : IPatchBuilder
{
    public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, PatchOptions patchOptions)
    {
        if (!patchOptions.HasFlag(PatchOptions.IncludeSprites))
        {
            return;
        }

        foreach (var file in overrideProvider.GetAllDataFilesInFolder(Path.Combine("graphics", "ikusa_map")))
        {
            if (file.IsOverride)
            {
                filesToPatch.Add(new FileToPatch(file.RomPath, file.File, FilePatchOptions.VariableLength));
            }
        }
    }
}
