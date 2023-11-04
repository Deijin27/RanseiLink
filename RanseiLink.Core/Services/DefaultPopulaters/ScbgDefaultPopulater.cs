using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using System.IO;

namespace RanseiLink.Core.Services.DefaultPopulaters;

[DefaultPopulater]
public class ScbgDefaultPopulater : IGraphicTypeDefaultPopulater
{
    public MetaSpriteType Id => MetaSpriteType.SCBG;
    public void ProcessExportedFiles(string defaultDataFolder, IGraphicsInfo gInfo)
    {
        var scbgInfo = (ScbgConstants)gInfo;
        string data = Path.Combine(defaultDataFolder, scbgInfo.Data);
        string info = Path.Combine(defaultDataFolder, scbgInfo.Info);
        bool tiled = true;
        string pngDir = Path.Combine(defaultDataFolder, scbgInfo.PngFolder);
        SCBGCollection.Load(data, info).SaveAsPngs(pngDir, tiled);

    }
}