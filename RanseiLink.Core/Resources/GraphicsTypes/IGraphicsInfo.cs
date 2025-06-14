using RanseiLink.Core.Services;
using System.Collections.Concurrent;
using System.Xml.Linq;

namespace RanseiLink.Core.Resources;

public interface IGraphicsInfo
{
    MetaSpriteType MetaType { get; }
    SpriteType Type { get; }
    string DisplayName { get; }
    bool FixedAmount { get; }
    string GetRelativeSpritePath(int id);
    int GetPaletteCapacity(int id);

    void GetFilesToPatch(GraphicsPatchContext context);
    void ProcessExportedFiles(string defaultDataFolder);
    List<SpriteFile> GetAllSpriteFiles(bool isOverride, string folder);
}

public record GraphicsPatchContext(
    ConcurrentBag<FileToPatch> FilesToPatch,
    IOverrideDataProvider OverrideDataProvider,
    IFallbackDataProvider FallbackDataProvider,
    string DefaultDataFolder,
    IServiceGetter ModServiceGetter
    );

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
    public abstract void ProcessExportedFiles(string defaultDataFolder);
    public abstract void GetFilesToPatch(GraphicsPatchContext context);
    public abstract List<SpriteFile> GetAllSpriteFiles(bool isOverride, string folder);
}