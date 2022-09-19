using RanseiLink.Core.Resources;
using System.Collections.Concurrent;

namespace RanseiLink.Core.Services.ModPatchBuilders
{
    public interface IGraphicTypePatchBuilder
    {
        void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, IGraphicsInfo gInfo);
    }
}
