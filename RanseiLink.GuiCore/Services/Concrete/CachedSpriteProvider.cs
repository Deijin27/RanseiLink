#nullable enable
using RanseiLink.Core.Graphics;
using RanseiLink.Core;
using RanseiLink.Core.Services;
using SixLabors.ImageSharp;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Models;
using RanseiLink.Core.Enums;

namespace RanseiLink.GuiCore.Services.Concrete;

public class CachedSpriteProvider : ICachedSpriteProvider
{
    private readonly IOverrideDataProvider _overrideDataProvider;
    private readonly IPathToImageConverter _pathToImageConverter;
    private readonly ISpriteService _spriteService;
    private readonly IMoveRangeService _moveRangeService;
    private readonly IGimmickRangeService _gimmickRangeService;
    private readonly Dictionary<int, object?> _cache = [];

    public CachedSpriteProvider(
        IOverrideDataProvider overrideDataProvider, 
        IPathToImageConverter pathToImageConverter,
        ISpriteService spriteService, 
        IMoveRangeService moveRangeService,
        IGimmickRangeService gimmickRangeService
        )
    {
        _overrideDataProvider = overrideDataProvider;
        _pathToImageConverter = pathToImageConverter;
        _spriteService = spriteService;
        _moveRangeService = moveRangeService;
        _gimmickRangeService = gimmickRangeService;
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
                var result = _pathToImageConverter.TryConvert(image);
                image.Dispose();
                return result;
            })
            .ToArray();
    }

    public object? GetMovePreview(Move model, MovePreviewOptions options = MovePreviewOptions.All)
    {
        using var img = _spriteService.GetMovePreview(model, _moveRangeService.Retrieve((int)model.Range), options);
        return _pathToImageConverter.TryConvert(img);
    }

    public object? GetGimmickPreview(Gimmick model, MovePreviewOptions options = MovePreviewOptions.All)
    {
        using var img = _spriteService.GetGimmickPreview(model, _gimmickRangeService.Retrieve((int)model.Range), options);
        return _pathToImageConverter.TryConvert(img);
    }

    public object? GetMoveRangePreview(MoveRangeId range)
    {
        using var img = _spriteService.GetMoveRangePreview(_moveRangeService.Retrieve((int)range));
        return _pathToImageConverter.TryConvert(img);
    }

    public object? GetGimmickRangePreview(GimmickRangeId range)
    {
        using var img = _spriteService.GetMoveRangePreview(_gimmickRangeService.Retrieve((int)range));
        return _pathToImageConverter.TryConvert(img);
    }

    public object? GetMoveRangePreview(MoveRange model)
    {
        using var img = _spriteService.GetMoveRangePreview(model);
        return _pathToImageConverter.TryConvert(img);
    }
}
