using System.Collections.Concurrent;
using RanseiLink.Core.Resources;

namespace RanseiLink.Core.Services.ModPatchBuilders;

[PatchBuilder]
public class AnimationPatchBuilder(IOverrideDataProvider overrideProvider) : IPatchBuilder
{
    public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, PatchOptions patchOptions)
    {
        if (!patchOptions.HasFlag(PatchOptions.IncludeSprites))
        {
            return;
        }

        foreach (var type in AnimationTypeInfoResource.All)
        {
            for (int i = 0; i <= type.MaxId; i++)
            {
                foreach (var relativePath in new[] { type.BackgroundRelativePath(i), type.AnimationRelativePath(i) })
                {
                    if (relativePath == null)
                    {
                        continue;
                    }
                    var file = overrideProvider.GetDataFile(relativePath);
                    if (!file.IsOverride)
                    {
                        continue;
                    }
                    filesToPatch.Add(new FileToPatch(file.RomPath, file.File, FilePatchOptions.VariableLength));
                }
            }
        }
    }
}