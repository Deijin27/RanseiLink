using RanseiLink.Core.Resources;
using System.Collections.Concurrent;

namespace RanseiLink.Core.Services.ModPatchBuilders;
public interface IMiscItemPatchBuilder
{
    MetaMiscItemId Id { get; }
    void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, MiscConstants gInfo, MiscItem miscItem, string pngFile);
}
