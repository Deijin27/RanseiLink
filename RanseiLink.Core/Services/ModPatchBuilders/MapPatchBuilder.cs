using System.Collections.Concurrent;

namespace RanseiLink.Core.Services.ModPatchBuilders;

[PatchBuilder]
public class MapPatchBuilder(ModInfo mod) : IPatchBuilder
{
    public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, PatchOptions patchOptions)
    {
        foreach (var mapFilePath in Directory.GetFiles(Path.Combine(mod.FolderPath, Constants.MapFolderPath)))
        {
            string mapRomPath = Path.Combine(Constants.MapFolderPath, Path.GetFileName(mapFilePath));
            filesToPatch.Add(new FileToPatch(mapRomPath, mapFilePath, FilePatchOptions.VariableLength));
        }
    }
}