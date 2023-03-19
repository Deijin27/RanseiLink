#nullable enable
using System.IO;
using System.Xml.Linq;

namespace RanseiLink.Core.Resources
{
    public class ScbgConstants : GroupedGraphicsInfo
    {
        public string Info { get; }
        public string Data { get; }
        public override string PngFolder { get; }
        public override int PaletteCapacity => 256;

        public ScbgConstants(MetaSpriteType metaType, XElement element) : base(metaType, element)
        {
            Info = FileUtil.NormalizePath(element.Element("Info")!.Value);
            Data = FileUtil.NormalizePath(element.Element("Data")!.Value);
            PngFolder = Path.Combine(Path.GetDirectoryName(Data)!, Path.GetFileNameWithoutExtension(Data) + "-Pngs");
        }
    }
}