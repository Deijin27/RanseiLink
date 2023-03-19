using RanseiLink.Core.Services;
using System.Xml.Linq;
using System;

namespace RanseiLink.Core.Resources
{
    public interface IGraphicsInfo
    {
        MetaSpriteType MetaType { get; }
        SpriteType Type { get; }
        string DisplayName { get; }
        bool FixedAmount { get; }
        string GetRelativeSpritePath(int id);
        int GetPaletteCapacity(int id);
    }

    public abstract class GraphicsInfo : IGraphicsInfo
    {
        public MetaSpriteType MetaType { get; }
        public SpriteType Type { get; }
        public string DisplayName { get; }
        public abstract bool FixedAmount { get; }

        public GraphicsInfo(MetaSpriteType metaType, XElement element)
        {
            MetaType = metaType;
            Type = (SpriteType)Enum.Parse(typeof(SpriteType), element.Attribute("Id")!.Value);
            DisplayName = element.Attribute("DisplayName")!.Value;
        }

        public abstract string GetRelativeSpritePath(int id);
        public abstract int GetPaletteCapacity(int id);
    }
}