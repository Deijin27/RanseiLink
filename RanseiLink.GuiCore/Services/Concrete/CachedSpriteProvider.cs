#nullable enable
using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.Services.Concrete;

public class CachedSpriteProvider : ICachedSpriteProvider
{
    private readonly IOverrideDataProvider _overrideDataProvider;
    private readonly IPathToImageConverter _pathToImageConverter;
    private readonly Dictionary<int, object?> _cache = [];

    public CachedSpriteProvider(IOverrideDataProvider overrideDataProvider, IPathToImageConverter pathToImageConverter)
    {
        _overrideDataProvider = overrideDataProvider;
        _pathToImageConverter = pathToImageConverter;
        _overrideDataProvider.SpriteModified += OverrideDataProvider_SpriteModified;
    }

    private void OverrideDataProvider_SpriteModified(object? sender, SpriteModifiedArgs e)
    {
        _cache.Remove(ResolveKey(e.Type, e.Id));
    }

    private static int ResolveKey(SpriteType type, int id)
    {
        return (int)type << 16 | id;
    }

    public object? GetSprite(SpriteType type, int id)
    {
        if (id < 0)
        {
            return null;
        }
        var file = _overrideDataProvider.GetSpriteFile(type, id);
        var imageSource = _pathToImageConverter.TryConvert(file.File);
        _cache[ResolveKey(type, id)] = imageSource;
        return imageSource;
    }
}
