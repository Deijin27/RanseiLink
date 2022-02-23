using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Nds;
using RanseiLink.Core.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RanseiLink.Core.Services.Concrete;

internal class FallbackSpriteProvider : IFallbackSpriteProvider
{
    private readonly string _graphicsProviderFolder;
    private readonly NdsFactory _ndsFactory;

    public FallbackSpriteProvider(string rootFolder, NdsFactory ndsFactory)
    {
        _ndsFactory = ndsFactory;
        _graphicsProviderFolder = Path.Combine(rootFolder, "DataProvider");
        Directory.CreateDirectory(_graphicsProviderFolder);
    }

    #region Populate

    public void Populate(string ndsFile, IProgress<ProgressInfo> progress = null)
    {
        // reset the graphics folder
        if (Directory.Exists(_graphicsProviderFolder))
        {
            progress?.Report(new ProgressInfo(StatusText:"Deleting Existing...", IsIndeterminate:true));
            Directory.Delete(_graphicsProviderFolder, true);
        }
        Directory.CreateDirectory(_graphicsProviderFolder);

        // populate
        progress?.Report(new ProgressInfo(StatusText: "Extracting files from rom...", IsIndeterminate: true));
        using var nds = _ndsFactory(ndsFile);
        nds.ExtractCopyOfDirectory(Constants.GraphicsFolderPath, _graphicsProviderFolder);
        var infos = GraphicsInfoResource.All;
        progress?.Report(new ProgressInfo(StatusText: "Converting Images...", IsIndeterminate: false, MaxProgress: infos.Count));
        int count = 0;
        foreach (var gfxInfo in infos)
        {
            switch (gfxInfo)
            {
                case StlConstants stlInfo:
                    UnpackStl(stlInfo);
                    break;
                case ScbgConstants scbgInfo:
                    UnpackScbg(scbgInfo);
                    break;
                case PkmdlConstants pkmdlInfo:
                    UnpackPkmdl(pkmdlInfo);
                    break;
                default:
                    throw new Exception($"Other types of {nameof(IGraphicsInfo)} not supported");
            }
            progress?.Report(new ProgressInfo(Progress: ++count));
        }
        progress?.Report(new ProgressInfo(StatusText: "Done!"));
    }

    private void UnpackStl(StlConstants stlInfo)
    {
        LINK.Unpack(Path.Combine(_graphicsProviderFolder, stlInfo.Link), Path.Combine(_graphicsProviderFolder, stlInfo.LinkFolder), true, 4);
        var ncer = NCER.Load(Path.Combine(_graphicsProviderFolder, stlInfo.Ncer));
        string data = Path.Combine(_graphicsProviderFolder, stlInfo.TexData ?? stlInfo.Data);
        string info = Path.Combine(_graphicsProviderFolder, stlInfo.TexInfo ?? stlInfo.Info);
        bool tiled = stlInfo.TexData == null;
        string pngDir = Path.Combine(_graphicsProviderFolder, stlInfo.PngFolder);
        STLCollection.Load(data, info).SaveAsPngs(pngDir, ncer, tiled);
    }

    private void UnpackScbg(ScbgConstants scbgInfo)
    {
        string data = Path.Combine(_graphicsProviderFolder, scbgInfo.Data);
        string info = Path.Combine(_graphicsProviderFolder, scbgInfo.Info);
        bool tiled = true;
        string pngDir = Path.Combine(_graphicsProviderFolder, scbgInfo.PngFolder);
        SCBGCollection.Load(data, info).SaveAsPngs(pngDir, tiled);
    }

    private void UnpackPkmdl(PkmdlConstants pkmdlInfo)
    {
        PokemonModelManager.UnpackModels(pkmdlInfo, _graphicsProviderFolder);
    }

    #endregion

    #region Provide

    public List<SpriteFile> GetAllSpriteFiles(SpriteType type)
    {
        string dir = Path.Combine(_graphicsProviderFolder, GraphicsInfoResource.Get(type).PngFolder);
        if (!Directory.Exists(dir))
        {
            return new List<SpriteFile>();
        }
        return Directory.GetFiles(dir)
            .Select(i => new SpriteFile(type, uint.Parse(Path.GetFileNameWithoutExtension(i)), i, IsOverride:false))
            .ToList();
    }

    public SpriteFile GetSpriteFile(SpriteType type, uint id)
    {
        return new SpriteFile(type, id, Path.Combine(_graphicsProviderFolder, GraphicsInfoResource.GetRelativeSpritePath(type, id)), false);
    }

    #endregion
}
