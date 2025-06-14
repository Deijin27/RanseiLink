using RanseiLink.Core.Services;
using System.Xml.Linq;

namespace RanseiLink.Core.Resources;

public static class GraphicsInfoResource
{
    private const string __graphicsInfoXml = "GraphicsInfo.xml";
    static GraphicsInfoResource()
    {
        using var stream = ResourceUtil.GetResourceStream(__graphicsInfoXml);
        XDocument doc = XDocument.Load(stream);
        var root = doc.Element("GraphicsInfo")!;
        foreach (var element in root.Elements())
        {
            var metaType = Enum.Parse<MetaSpriteType>(element.Name.ToString());
            GraphicsInfo info = metaType switch
            {
                MetaSpriteType.STL => new StlGraphicsInfo(metaType, element),
                MetaSpriteType.SCBG => new ScbgGraphicsInfo(metaType, element),
                MetaSpriteType.PKMDL => new PkmdlGraphicsInfo(metaType, element),
                MetaSpriteType.Misc => new MiscGraphicsInfo(metaType, element),
                _ => throw new Exception("Invalid element in GraphicsInfo.xml"),
            };
            __all[info.Type] = info;
        }
    }

    private static readonly Dictionary<SpriteType, IGraphicsInfo> __all = [];

    public static IReadOnlyCollection<IGraphicsInfo> All => __all.Values;
    public static IGraphicsInfo Get(SpriteType type)
    {
        return __all[type];
    }
}