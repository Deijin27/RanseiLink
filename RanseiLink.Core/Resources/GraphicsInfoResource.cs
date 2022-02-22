using RanseiLink.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace RanseiLink.Core.Resources;

public class StlConstants : IGraphicsInfo
{
    public SpriteType Type { get; init; }
    public string DisplayName { get; init; }
    public int? Width { get; init; }
    public int? Height { get; init; }
    public string TexInfo { get; init; }
    public string TexData { get; init; }
    public string Info { get; init; }
    public string Data { get; init; }
    public string Link { get; init; }

    public string LinkFolder => Path.Combine(Path.GetDirectoryName(Link), "UnpackedLink");
    public string Ncer => Path.Combine(LinkFolder, "0002.ncer");
    private string _pngFolder;
    public string PngFolder => _pngFolder ??= Path.Combine(Path.GetDirectoryName(TexData ?? Data), Path.GetFileNameWithoutExtension(TexData ?? Data) + "-Pngs");

    public int PaletteCapacity => 256;
}

public class ScbgConstants : IGraphicsInfo
{
    public SpriteType Type { get; init; }
    public string DisplayName { get; init; }
    public int? Width { get; init; }
    public int? Height { get; init; }
    public string Info { get; init; }
    public string Data { get; init; }
    private string _pngFolder;
    public string PngFolder => _pngFolder ??= Path.Combine(Path.GetDirectoryName(Data), Path.GetFileNameWithoutExtension(Data) + "-Pngs");

    public int PaletteCapacity => 256;
}

public class PkmdlConstants : IGraphicsInfo
{
    public SpriteType Type { get; init; }
    public string DisplayName { get; init; }
    public int? Width { get; init; }
    public int? Height { get; init; }
    public string TEXLink { get; init; }
    public string TEXLinkFolder => Path.Combine(Path.GetDirectoryName(TEXLink), $"TEXLink-Unpacked");
    public string ATXLink { get; init; }
    public string ATXLinkFolder => Path.Combine(Path.GetDirectoryName(ATXLink), $"ATXLink-Unpacked");
    public string DTXLink { get; init; }
    public string DTXLinkFolder => Path.Combine(Path.GetDirectoryName(DTXLink), $"DTXLink-Unpacked");
    public string PACLink { get; init; }
    public string PACLinkFolder => Path.Combine(Path.GetDirectoryName(PACLink), $"PACLink-Unpacked");

    private string _pngFolder;
    public string PngFolder => _pngFolder ??= Path.Combine(Path.GetDirectoryName(TEXLink), "Pngs");

    public int PaletteCapacity => 16;
}

public interface IGraphicsInfo
{
    public SpriteType Type { get; init; }
    public string DisplayName { get; init; }
    public int? Width { get; init; }
    public int? Height { get; init; }
    public string PngFolder { get; }
    public int PaletteCapacity { get; }
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
            IGraphicsInfo info = element.Name.ToString() switch
            {
                "STL" => new StlConstants()
                {
                    Type = Enum.Parse<SpriteType>(element.Attribute("Id").Value),
                    DisplayName = element.Element("DisplayName").Value,
                    TexInfo = FileUtil.NormalizePath(element.Element("TexInfo")?.Value),
                    TexData = FileUtil.NormalizePath(element.Element("TexData")?.Value),
                    Info = FileUtil.NormalizePath(element.Element("Info")?.Value),
                    Data = FileUtil.NormalizePath(element.Element("Data")?.Value),
                    Link = FileUtil.NormalizePath(element.Element("Link").Value),
                    Width = int.TryParse(element.Element("Width")?.Value, out int resultWidth) ? resultWidth : null,
                    Height = int.TryParse(element.Element("Height")?.Value, out int resultHeight) ? resultHeight : null
                },
                "SCBG" => new ScbgConstants()
                {
                    Type = Enum.Parse<SpriteType>(element.Attribute("Id").Value),
                    DisplayName = element.Element("DisplayName").Value,
                    Info = FileUtil.NormalizePath(element.Element("Info").Value),
                    Data = FileUtil.NormalizePath(element.Element("Data").Value),
                    Width = int.TryParse(element.Element("Width")?.Value, out int resultWidth) ? resultWidth : null,
                    Height = int.TryParse(element.Element("Height")?.Value, out int resultHeight) ? resultHeight : null
                },
                "PKMDL" => new PkmdlConstants()
                {
                    Type = Enum.Parse<SpriteType>(element.Attribute("Id").Value),
                    DisplayName = element.Element("DisplayName").Value,
                    TEXLink = FileUtil.NormalizePath(element.Element("TEXLink").Value),
                    ATXLink = FileUtil.NormalizePath(element.Element("ATXLink").Value),
                    DTXLink = FileUtil.NormalizePath(element.Element("DTXLink").Value),
                    PACLink = FileUtil.NormalizePath(element.Element("PACLink").Value),
                    Width = int.TryParse(element.Element("Width")?.Value, out int resultWidth) ? resultWidth : null,
                    Height = int.TryParse(element.Element("Height")?.Value, out int resultHeight) ? resultHeight : null
                },
                _ => throw new Exception("Invalid element in GraphicsInfo.xml")
            };
            _all[info.Type] = info;
        }
    }

    private static readonly Dictionary<SpriteType, IGraphicsInfo> _all = new Dictionary<SpriteType, IGraphicsInfo>();

    public static IReadOnlyCollection<IGraphicsInfo> All => _all.Values;
    public static IGraphicsInfo Get(SpriteType type)
    {
        return _all[type];
    }

    public static string GetRelativeSpritePath(SpriteType type, uint id)
    {
        string idString = id.ToString().PadLeft(4, '0');
        string dir = GraphicsInfoResource.Get(type).PngFolder;
        return Path.Combine(dir, idString + ".png");
    }
}
