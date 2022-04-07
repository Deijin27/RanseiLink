using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Services.Concrete;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace RanseiLink.Core.Services.ModPatchBuilders;

public class StlPatchBuilder : IGraphicTypePatchBuilder
{
    private readonly IOverrideSpriteProvider _overrideSpriteProvider;
    private readonly string _graphicsProviderFolder = Constants.DefaultDataProviderFolder;
    public StlPatchBuilder(IOverrideSpriteProvider overrideSpriteProvider)
    {
        _overrideSpriteProvider = overrideSpriteProvider;
    }

    public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, IGraphicsInfo gInfo)
    {
        if (gInfo is not StlConstants stlInfo)
        {
            return;
        }

        var spriteFiles = _overrideSpriteProvider.GetAllSpriteFiles(stlInfo.Type);
        if (!spriteFiles.Any(i => i.IsOverride))
        {
            return;
        }

        string[] filesToPack = spriteFiles.Select(i => i.File).ToArray();
        var ncer = NCER.Load(Path.Combine(_graphicsProviderFolder, stlInfo.Ncer));
        if (stlInfo.TexInfo != null)
        {
            string texData = Path.GetTempFileName();
            string texInfo = Path.GetTempFileName();
            STLCollection
                .LoadPngs(filesToPack, ncer, tiled: false)
                .Save(texData, texInfo);
            filesToPatch.Add(new FileToPatch(stlInfo.TexInfo, texInfo, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
            filesToPatch.Add(new FileToPatch(stlInfo.TexData, texData, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
        }

        if (stlInfo.Info != null)
        {
            string info = Path.GetTempFileName();
            string data = Path.GetTempFileName();
            STLCollection
                .LoadPngs(filesToPack, ncer, tiled: true)
                .Save(data, info);
            filesToPatch.Add(new FileToPatch(stlInfo.Info, info, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
            filesToPatch.Add(new FileToPatch(stlInfo.Data, data, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
        }
    }
}
