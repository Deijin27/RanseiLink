using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using System.IO;

namespace RanseiLink.Core.Services.DefaultPopulaters
{
    public class StlDefaultPopulater : IGraphicTypeDefaultPopulater
    {
        public void ProcessExportedFiles(string defaultDataFolder, IGraphicsInfo gInfo)
        {
            if (!(gInfo is StlConstants stlInfo))
            {
                return;
            }
            LINK.Unpack(Path.Combine(defaultDataFolder, stlInfo.Link), Path.Combine(defaultDataFolder, stlInfo.LinkFolder), true, 4);
            var ncer = NCER.Load(Path.Combine(defaultDataFolder, stlInfo.Ncer));
            string data = Path.Combine(defaultDataFolder, stlInfo.TexData ?? stlInfo.Data);
            string info = Path.Combine(defaultDataFolder, stlInfo.TexInfo ?? stlInfo.Info);
            bool tiled = stlInfo.TexData == null;
            string pngDir = Path.Combine(defaultDataFolder, stlInfo.PngFolder);
            STLCollection.Load(data, info).SaveAsPngs(pngDir, ncer, tiled);
        }
    }

}