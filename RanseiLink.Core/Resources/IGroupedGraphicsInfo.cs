using System.Xml.Linq;

namespace RanseiLink.Core.Resources
{
    public interface IGroupedGraphicsInfo : IGraphicsInfo
    {
        string PngFolder { get; }
        int? Width { get; }
        int? Height { get; }
        bool StrictWidth { get; }
        bool StrictHeight { get; }
        int PaletteCapacity { get; }
    }

    public abstract class GroupedGraphicsInfo : GraphicsInfo, IGroupedGraphicsInfo
    {
        public int? Width { get; }
        public int? Height { get; }
        public bool StrictWidth { get; }
        public bool StrictHeight { get; }
        public override bool FixedAmount { get; }
        public abstract string PngFolder { get; }
        public abstract int PaletteCapacity { get; }

        public GroupedGraphicsInfo(MetaSpriteType metaType, XElement element) : base(metaType, element)
        {
            var widthElement = element.Element("Width");
            if (widthElement != null)
            {
                Width = int.Parse(widthElement.Value);
                StrictWidth = widthElement.Attribute("Strict")?.Value == "true";
            }
            var heightElement = element.Element("Height");
            if (heightElement != null)
            {
                Height = int.Parse(heightElement.Value);
                StrictHeight = heightElement.Attribute("Strict")?.Value == "true";
            }
            FixedAmount = element.Element("FixedAmount")?.Value == "true";
        }

        public override string GetRelativeSpritePath(int id)
        {
            string idString = id.ToString().PadLeft(4, '0');
            return Path.Combine(PngFolder, idString + ".png");
        }

        public override int GetPaletteCapacity(int id)
        {
            return PaletteCapacity;
        }
    }
}