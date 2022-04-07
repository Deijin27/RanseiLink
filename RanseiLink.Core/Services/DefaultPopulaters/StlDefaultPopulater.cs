using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using System.IO;

namespace RanseiLink.Core.Services.DefaultPopulaters;

public class StlDefaultPopulater : IGraphicTypeDefaultPopulater
{
    private readonly string _graphicsProviderFolder = Constants.DefaultDataProviderFolder;
    public StlDefaultPopulater()
    {
    }

    public void ProcessExportedFiles(IGraphicsInfo gInfo)
    {
        if (gInfo is not StlConstants stlInfo)
        {
            return;
        }
        LINK.Unpack(Path.Combine(_graphicsProviderFolder, stlInfo.Link), Path.Combine(_graphicsProviderFolder, stlInfo.LinkFolder), true, 4);
        var ncer = NCER.Load(Path.Combine(_graphicsProviderFolder, stlInfo.Ncer));
        string data = Path.Combine(_graphicsProviderFolder, stlInfo.TexData ?? stlInfo.Data);
        string info = Path.Combine(_graphicsProviderFolder, stlInfo.TexInfo ?? stlInfo.Info);
        bool tiled = stlInfo.TexData == null;
        string pngDir = Path.Combine(_graphicsProviderFolder, stlInfo.PngFolder);
        STLCollection.Load(data, info).SaveAsPngs(pngDir, ncer, tiled);
    }
}

