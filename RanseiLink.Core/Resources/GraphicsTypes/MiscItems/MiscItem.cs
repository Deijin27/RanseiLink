using System.Xml.Linq;

namespace RanseiLink.Core.Resources;

public abstract class MiscItem
{
    public MetaMiscItemId MetaId { get; }
    public int Id { get; }
    public int PaletteCapacity { get; }
    public abstract string PngFile { get; }

    public MiscItem(MetaMiscItemId metaId, int id, XElement element)
    {
        MetaId = metaId;
        Id = id;
        var palAttr = element.Attribute("PaletteCapacity");
        PaletteCapacity = palAttr != null ? int.Parse(palAttr.Value) : 256;
    }

    public abstract void ProcessExportedFiles(string defaultDataFolder, MiscGraphicsInfo gInfo);
    public abstract void GetFilesToPatch(GraphicsPatchContext context, MiscGraphicsInfo gInfo, string pngFile);
}
