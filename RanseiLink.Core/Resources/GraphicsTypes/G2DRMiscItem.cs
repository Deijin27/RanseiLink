using System.Xml.Linq;

namespace RanseiLink.Core.Resources;

public abstract class G2DRMiscItem : MiscItem
{
    public string Link { get; }
    public string LinkFolder { get; }
    public override string PngFile { get; }

    public G2DRMiscItem(MetaMiscItemId metaId, int id, XElement element) : base(metaId, id, element)
    {
        Link = element.Attribute("Link")!.Value;
        LinkFolder = Path.Combine(Path.GetDirectoryName(Link)!, Path.GetFileNameWithoutExtension(Link) + "-Unpacked");
        PngFile = Path.Combine(LinkFolder, "Image.png");
    }
}
