using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace RanseiLink.Core.Services.ModPatchBuilders;

[PatchBuilder]
public class ScbgPatchBuilder(IOverrideDataProvider overrideSpriteProvider) : IGraphicTypePatchBuilder
{
    public MetaSpriteType Id => MetaSpriteType.SCBG;

    public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, IGraphicsInfo gInfo)
    {
        var scbgInfo = (ScbgConstants)gInfo;

        var spriteFiles = overrideSpriteProvider.GetAllSpriteFiles(scbgInfo.Type);
        if (!spriteFiles.Any(i => i.IsOverride))
        {
            return;
        }

        string[] filesToPack = spriteFiles.Select(i => i.File).ToArray();
        string data = Path.GetTempFileName();
        string info = Path.GetTempFileName();

        SCBGCollection
            .LoadPngs(filesToPack, tiled: true)
            .Save(data, info);

        filesToPatch.Add(new FileToPatch(scbgInfo.Data, data, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
        filesToPatch.Add(new FileToPatch(scbgInfo.Info, info, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));

    }
}