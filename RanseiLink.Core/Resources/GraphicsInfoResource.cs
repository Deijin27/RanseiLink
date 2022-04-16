using RanseiLink.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace RanseiLink.Core.Resources
{
    public class StlConstants : BaseGraphicsInfo
    {
        public string TexInfo { get; set; }
        public string TexData { get; set; }
        public string Info { get; set; }
        public string Data { get; set; }
        public string Link { get; set; }

        public string LinkFolder => Path.Combine(Path.GetDirectoryName(Link), "UnpackedLink");
        public string Ncer => Path.Combine(LinkFolder, "0002.ncer");
        private string _pngFolder;
        public override string PngFolder => _pngFolder ?? (_pngFolder = Path.Combine(Path.GetDirectoryName(TexData ?? Data), Path.GetFileNameWithoutExtension(TexData ?? Data) + "-Pngs"));

        public override int PaletteCapacity => 256;
    }

    public class ScbgConstants : BaseGraphicsInfo
    {
        public string Info { get; set; }
        public string Data { get; set; }
        private string _pngFolder;
        public override string PngFolder => _pngFolder ?? (_pngFolder = Path.Combine(Path.GetDirectoryName(Data), Path.GetFileNameWithoutExtension(Data) + "-Pngs"));

        public override int PaletteCapacity => 256;
    }

    public class PkmdlConstants : BaseGraphicsInfo
    {
        public string TEXLink { get; set; }
        public string TEXLinkFolder => Path.Combine(Path.GetDirectoryName(TEXLink), $"TEXLink-Unpacked");
        public string ATXLink { get; set; }
        public string ATXLinkFolder => Path.Combine(Path.GetDirectoryName(ATXLink), $"ATXLink-Unpacked");
        public string DTXLink { get; set; }
        public string DTXLinkFolder => Path.Combine(Path.GetDirectoryName(DTXLink), $"DTXLink-Unpacked");
        public string PACLink { get; set; }
        public string PACLinkFolder => Path.Combine(Path.GetDirectoryName(PACLink), $"PACLink-Unpacked");

        private string _pngFolder;
        public override string PngFolder => _pngFolder ?? (_pngFolder = Path.Combine(Path.GetDirectoryName(TEXLink), "Pngs"));

        public override int PaletteCapacity => 16;
    }

    public interface IGraphicsInfo
    {
        SpriteType Type { get; }
        string DisplayName { get; }
        int? Width { get; }
        int? Height { get; }
        bool StrictWidth { get; }
        bool StrictHeight { get; }
        string PngFolder { get; }
        int PaletteCapacity { get; }
        bool FixedAmount { get; }
    }

    public abstract class BaseGraphicsInfo : IGraphicsInfo
    {
        public SpriteType Type { get; set; }
        public string DisplayName { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public bool StrictWidth { get; set; }
        public bool StrictHeight { get; set; }
        public bool FixedAmount { get; set; }
        public abstract string PngFolder { get; }
        public abstract int PaletteCapacity { get; }
    }

    public static class GraphicsInfoResource
    {
        private const string _graphicsInfoXml = "RanseiLink.Core.Resources.GraphicsInfo.xml";
        static GraphicsInfoResource()
        {
            XDocument doc;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_graphicsInfoXml))
            {
                doc = XDocument.Load(stream);
            }

            var root = doc.Element("GraphicsInfo");
            foreach (var element in root.Elements())
            {
                BaseGraphicsInfo info;
                switch (element.Name.ToString())
                {
                    case "STL": 
                        info = new StlConstants()
                        {
                            TexInfo = FileUtil.NormalizePath(element.Element("TexInfo")?.Value),
                            TexData = FileUtil.NormalizePath(element.Element("TexData")?.Value),
                            Info = FileUtil.NormalizePath(element.Element("Info")?.Value),
                            Data = FileUtil.NormalizePath(element.Element("Data")?.Value),
                            Link = FileUtil.NormalizePath(element.Element("Link").Value),
                        };
                        break;
                    case "SCBG":
                        info = new ScbgConstants()
                        {
                            Info = FileUtil.NormalizePath(element.Element("Info").Value),
                            Data = FileUtil.NormalizePath(element.Element("Data").Value),
                        };
                        break;
                    case "PKMDL":
                        info = new PkmdlConstants()
                        {
                            TEXLink = FileUtil.NormalizePath(element.Element("TEXLink").Value),
                            ATXLink = FileUtil.NormalizePath(element.Element("ATXLink").Value),
                            DTXLink = FileUtil.NormalizePath(element.Element("DTXLink").Value),
                            PACLink = FileUtil.NormalizePath(element.Element("PACLink").Value),
                        };
                        break;
                    default:
                        throw new Exception("Invalid element in GraphicsInfo.xml");
                }
                // Things in all image formats
                info.Type = (SpriteType)Enum.Parse(typeof(SpriteType), element.Attribute("Id").Value);
                info.DisplayName = element.Element("DisplayName").Value;
                var widthElement = element.Element("Width");
                if (widthElement != null)
                {
                    info.Width = int.TryParse(widthElement.Value, out int resultWidth) ? (int?)resultWidth : null;
                    info.StrictWidth = widthElement.Attribute("Strict")?.Value == "true";
                }
                var heightElement = element.Element("Height");
                if (heightElement != null)
                {
                    info.Height = int.TryParse(heightElement.Value, out int resultHeight) ? (int?)resultHeight : null;
                    info.StrictHeight = heightElement.Attribute("Strict")?.Value == "true";
                }
                info.FixedAmount = element.Element("FixedAmount")?.Value == "true";

                _all[info.Type] = info;
            }
        }

        private static readonly Dictionary<SpriteType, IGraphicsInfo> _all = new Dictionary<SpriteType, IGraphicsInfo>();

        public static IReadOnlyCollection<IGraphicsInfo> All => _all.Values;
        public static IGraphicsInfo Get(SpriteType type)
        {
            return _all[type];
        }

        public static string GetRelativeSpritePath(SpriteType type, int id)
        {
            string idString = id.ToString().PadLeft(4, '0');
            string dir = GraphicsInfoResource.Get(type).PngFolder;
            return Path.Combine(dir, idString + ".png");
        }
    }
}