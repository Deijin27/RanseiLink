#nullable enable
using RanseiLink.Core.Graphics;
using RanseiLink.Core;
using RanseiLink.Core.Services;
using SixLabors.ImageSharp;

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
        object? imageSource;
        if (type == SpriteType.IconInstS)
        {
            imageSource = GetIconInstS(id);
        }
        else
        {
            imageSource = LoadImage(type, id);
        }
        return imageSource;
    }

    object? LoadImage(SpriteType type, int id)
    {
        var key = ResolveKey(type, id);
        if (_cache.TryGetValue(key, out var sprite))
        {
            return sprite;
        }
        var file = _overrideDataProvider.GetSpriteFile(type, id);
        var img = _pathToImageConverter.TryConvert(file.File);
        _cache[key] = img;
        return img;
    }

    private object? GetIconInstS(int id)
    {
        const int dim = 32;
        var img = LoadImage(SpriteType.IconInstS, 0);
        return _pathToImageConverter.TryCrop(img, 0, dim * id, dim, dim);
    }

    public IReadOnlyList<object?> GetClusterImages(string linkPath, int[] clusters)
    {
        var file = _overrideDataProvider.GetDataFile(FileUtil.NormalizePath(linkPath));

        if (!File.Exists(file.File))
        {
            return new object?[clusters.Length];
        }

        var g2dr = G2DR.LoadCellImgFromFile(file.File);
        var settings = new CellImageSettings(PositionRelativeTo.MinCell);

        return NitroImageUtil.NcerToMultipleImages(clusters, g2dr.Ncer, g2dr.Ncgr, g2dr.Nclr, settings)
            .Select(image =>
            {
                var temp = Path.GetTempFileName();
                image.SaveAsPng(temp);
                image.Dispose();
                var result = _pathToImageConverter.TryConvert(temp);
                File.Delete(temp);
                return result;
            })
            .ToArray();
    }
}
