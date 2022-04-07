using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using System.IO;

namespace RanseiLink.Core.Services.DefaultPopulaters;

public class ScbgDefaultPopulater : IGraphicTypeDefaultPopulater
{
    private readonly string _graphicsProviderFolder = Constants.DefaultDataProviderFolder;
    public ScbgDefaultPopulater()
    {
    }

    public void ProcessExportedFiles(IGraphicsInfo gInfo)
    {
        if (gInfo is not ScbgConstants scbgInfo)
        {
            return;
        }
        string data = Path.Combine(_graphicsProviderFolder, scbgInfo.Data);
        string info = Path.Combine(_graphicsProviderFolder, scbgInfo.Info);
        bool tiled = true;
        string pngDir = Path.Combine(_graphicsProviderFolder, scbgInfo.PngFolder);
        SCBGCollection.Load(data, info).SaveAsPngs(pngDir, tiled);
        
    }
}

