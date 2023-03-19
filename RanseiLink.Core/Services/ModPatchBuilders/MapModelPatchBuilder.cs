using System.Collections.Concurrent;
using System.IO;

namespace RanseiLink.Core.Services.ModPatchBuilders;

public class MapModelPatchBuilder : IPatchBuilder
{
    private readonly IOverrideDataProvider _overrideProvider;
    public MapModelPatchBuilder(IOverrideDataProvider overrideProvider)
    {
        _overrideProvider = overrideProvider;
    }

    public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, PatchOptions patchOptions)
    {
        if (!patchOptions.HasFlag(PatchOptions.IncludeSprites))
        {
            return;
        }

        foreach (var file in _overrideProvider.GetAllDataFilesInFolder(Path.Combine("graphics", "ikusa_map")))
        {
            if (file.IsOverride)
            {
                filesToPatch.Add(new FileToPatch(file.RomPath, file.File, FilePatchOptions.VariableLength));
            }
        }
    }
}
