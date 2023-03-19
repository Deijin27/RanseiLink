#nullable enable
using System.Collections.Concurrent;
using System.IO;

namespace RanseiLink.Core.Services.ModPatchBuilders;

public class MapPatchBuilder : IPatchBuilder
{
    private readonly ModInfo _mod;
    public MapPatchBuilder(ModInfo mod)
    {
        _mod = mod;
    }
    public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, PatchOptions patchOptions)
    {
        foreach (var mapFilePath in Directory.GetFiles(Path.Combine(_mod.FolderPath, Constants.MapFolderPath)))
        {
            string mapRomPath = Path.Combine(Constants.MapFolderPath, Path.GetFileName(mapFilePath));
            filesToPatch.Add(new FileToPatch(mapRomPath, mapFilePath, FilePatchOptions.VariableLength));
        }
    }
}