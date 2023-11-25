using RanseiLink.Core.Archive;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RanseiLink.Core.Resources
{
    public enum MetaMiscItemId
    {
        NCER,
        NSCR,
        IconInstS
    }

    public class MiscConstants : GraphicsInfo
    {
        public MiscItem[] Items { get; }

        public override bool FixedAmount => true;

        public MiscConstants(MetaSpriteType metaType, XElement element) : base(metaType, element)
        {
            Items = element.Elements().Select((miscItemElement, id) =>
            {
                var metaId = Enum.Parse<MetaMiscItemId>(miscItemElement.Name.ToString());
                return metaId switch
                {
                    MetaMiscItemId.NSCR or MetaMiscItemId.NCER => (MiscItem)new G2DRMiscItem(metaId, id, miscItemElement),
                    MetaMiscItemId.IconInstS => new BuildingIconSmallMiscItem(metaId, id, miscItemElement),
                    _ => throw new Exception("Invalid misc item element in GraphicsInfo.xml"),
                };
            }).ToArray();
        }

        public override string GetRelativeSpritePath(int id)
        {
            return Items[id].PngFile;
        }

        public override int GetPaletteCapacity(int id)
        {
            return Items[id].PaletteCapacity;
        }
    }

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
    }

    public class G2DRMiscItem : MiscItem
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

    public class BuildingIconSmallMiscItem : MiscItem
    {
        public string ContainingFolder { get; }
        public override string PngFile { get; }

        public BuildingIconSmallMiscItem(MetaMiscItemId metaId, int id, XElement element) : base(metaId, id, element)
        {
            ContainingFolder = element.Attribute("ContainingFolder")!.Value;
            PngFile = Path.Combine(ContainingFolder, "Image.png");
        }
    }
}