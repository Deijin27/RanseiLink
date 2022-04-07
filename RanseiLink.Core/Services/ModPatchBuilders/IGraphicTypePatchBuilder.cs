using RanseiLink.Core.Resources;
using RanseiLink.Core.Services.Concrete;
using System.Collections.Concurrent;

namespace RanseiLink.Core.Services.ModPatchBuilders;

public interface IGraphicTypePatchBuilder
{
    public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, IGraphicsInfo gInfo);
}
