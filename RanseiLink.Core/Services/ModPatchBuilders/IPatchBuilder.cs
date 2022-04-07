using RanseiLink.Core.Services.Concrete;
using System.Collections.Concurrent;

namespace RanseiLink.Core.Services.ModPatchBuilders;

public interface IPatchBuilder
{
    public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, PatchOptions patchOptions);
}
