using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using System.IO;

namespace RanseiLink.Core.Services.DefaultPopulaters;

public class ScbgDefaultPopulater : IGraphicTypeDefaultPopulater
{
    public void ProcessExportedFiles(string defaultDataFolder, IGraphicsInfo gInfo)
    {
        if (gInfo.MetaType != MetaSpriteType.SCBG)
        {
            return;
        }
        var scbgInfo = (ScbgConstants)gInfo;
        string data = Path.Combine(defaultDataFolder, scbgInfo.Data);
        string info = Path.Combine(defaultDataFolder, scbgInfo.Info);
        bool tiled = true;
        string pngDir = Path.Combine(defaultDataFolder, scbgInfo.PngFolder);
        SCBGCollection.Load(data, info).SaveAsPngs(pngDir, tiled);

    }
}