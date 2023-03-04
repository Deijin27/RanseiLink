using RanseiLink.Core.Services;
using RanseiLink.ValueConverters;
using System.Collections.Generic;
using System.Windows.Media;

namespace RanseiLink.Services.Concrete;

public class CachedSpriteProvider : ICachedSpriteProvider
{
    private readonly IOverrideDataProvider _overrideDataProvider;
    private readonly Dictionary<int, ImageSource> _cache = new();

    public CachedSpriteProvider(IOverrideDataProvider overrideDataProvider)
    {
        _overrideDataProvider = overrideDataProvider;

        _overrideDataProvider.SpriteModified += OverrideDataProvider_SpriteModified;
    }

    private void OverrideDataProvider_SpriteModified(object sender, SpriteModifiedArgs e)
    {
        _cache.Remove(ResolveKey(e.Type, e.Id));
    }

    private static int ResolveKey(SpriteType type, int id)
    {
        return (int)type << 16 | id;
    }

    public ImageSource GetSprite(SpriteType type, int id)
    {
        if (id < 0)
        {
            return null;
        }
        var file = _overrideDataProvider.GetSpriteFile(type, id);
        PathToImageSourceConverter.TryConvert(file.File, out var imageSource);
        _cache[ResolveKey(type, id)] = imageSource;
        return imageSource;
    }
}
