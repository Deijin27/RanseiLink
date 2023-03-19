#nullable enable
using System.IO;
using System.Xml.Linq;

namespace RanseiLink.Core.Resources
{
    public class PkmdlConstants : GroupedGraphicsInfo
    {
        public string TEXLink { get; }
        public string TEXLinkFolder { get; }
        public string ATXLink { get; }
        public string ATXLinkFolder { get; }
        public string DTXLink { get; }
        public string DTXLinkFolder { get; }
        public string PACLink { get; }
        public string PACLinkFolder { get; }
        public override string PngFolder { get; }
        public override int PaletteCapacity => 16;

        public PkmdlConstants(MetaSpriteType metaType, XElement element) : base(metaType, element)
        {
            TEXLink = FileUtil.NormalizePath(element.Element("TEXLink")!.Value);
            ATXLink = FileUtil.NormalizePath(element.Element("ATXLink")!.Value);
            DTXLink = FileUtil.NormalizePath(element.Element("DTXLink")!.Value);
            PACLink = FileUtil.NormalizePath(element.Element("PACLink")!.Value);

            TEXLinkFolder = Path.Combine(Path.GetDirectoryName(TEXLink)!, $"TEXLink-Unpacked");
            ATXLinkFolder = Path.Combine(Path.GetDirectoryName(ATXLink)!, $"ATXLink-Unpacked");
            DTXLinkFolder = Path.Combine(Path.GetDirectoryName(DTXLink)!, $"DTXLink-Unpacked");
            PACLinkFolder = Path.Combine(Path.GetDirectoryName(PACLink)!, $"PACLink-Unpacked");

            PngFolder = Path.Combine(Path.GetDirectoryName(TEXLink)!, "Pngs");
        }
    }
}