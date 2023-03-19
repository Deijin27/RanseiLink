#nullable enable
using System;
using System.IO;
using System.Xml.Linq;

namespace RanseiLink.Core.Resources
{
    public class StlConstants : GroupedGraphicsInfo
    {
        public string? TexInfo { get; }
        public string? TexData { get; }
        public string? Info { get; }
        public string? Data { get; }
        public string Link { get; }
        public string LinkFolder { get; }
        public string Ncer { get; }
        public override string PngFolder { get; }
        public override int PaletteCapacity => 256;

        public StlConstants(MetaSpriteType metaType, XElement element) : base(metaType, element)
        {
            TexInfo = FileUtil.NormalizePath(element.Element("TexInfo")?.Value);
            TexData = FileUtil.NormalizePath(element.Element("TexData")?.Value);
            Info = FileUtil.NormalizePath(element.Element("Info")?.Value);
            Data = FileUtil.NormalizePath(element.Element("Data")?.Value);
            Link = FileUtil.NormalizePath(element.Element("Link")!.Value);
            LinkFolder = Path.Combine(Path.GetDirectoryName(Link)!, "UnpackedLink");
            Ncer = Path.Combine(LinkFolder, "0002.ncer");
            string png;
            if (TexData != null)
            {
                png = TexData;
                if (TexInfo == null) throw new Exception();
            }
            else if (Data != null)
            {
                png = Data;
                if (Info == null) throw new Exception();
            }
            else
            {
                throw new Exception();
            }
            PngFolder = Path.Combine(Path.GetDirectoryName(png)!, Path.GetFileNameWithoutExtension(png) + "-Pngs");
        }
    }
}