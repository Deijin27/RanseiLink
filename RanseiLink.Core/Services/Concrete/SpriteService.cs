using RanseiLink.Core.Enums;
using RanseiLink.Core.Graphics;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using RanseiLink.Core.Models;
using RanseiLink.Core.Resources;

namespace RanseiLink.Core.Services.Concrete;

internal class SpriteService : ISpriteService
{

    private enum MoveIcons
    {
        PowerStar = 92,
        MoveRangeArrow = 93,
        MoveRangeGlowOrange = 94,
        MoveRangeGlowBlue = 95,
        MoveRangeGlowRed = 96,
    }

    private readonly IOverrideDataProvider _overrideDataProvider;
    private readonly Dictionary<MoveIcons, Image<Rgba32>> _cachedImages = [];
    private readonly Dictionary<TypeId, Image<Rgba32>> _typeImages = [];
    private readonly Image<Rgba32> _background;
    private readonly Image<Rgba32> _overlay;
    public SpriteService(IOverrideDataProvider overrideDataProvider)
    {
        _overrideDataProvider = overrideDataProvider;
        var moveIcons = _overrideDataProvider.GetDataFile("graphics/common/11_03_parts_ikusamap_up.G2DR");
        var link = G2DR.LoadCellImgFromFile(moveIcons.File);
        var iconIds = Enum.GetValues<MoveIcons>();
        var images = NitroImageUtil.NcerToMultipleImages(iconIds.Select(x => (int)x).ToArray(), link.Ncer, link.Ncgr, link.Nclr, new());
        foreach (var (icon, image) in iconIds.Zip(images))
        {
            _cachedImages[icon] = image;
        }
        _background = ResourceUtil.GetResourceImage("move_bg.png");
        _overlay = ResourceUtil.GetResourceImage("move_overlay.png");

        var typeIds = EnumUtil.GetValuesExceptDefaults<TypeId>();
        var typeIconLink = G2DR.LoadCellImgFromFile(_overrideDataProvider.GetDataFile("graphics/common/11_01_parts_usual_up.G2DR").File);
        var typeImages = NitroImageUtil.NcerToMultipleImages(typeIds.Select(x => (int)x).ToArray(), 
            typeIconLink.Ncer, typeIconLink.Ncgr, typeIconLink.Nclr, new());
        foreach (var (type, image) in typeIds.Zip(typeImages))
        {
            _typeImages[type] = image;
        }
    }

    private void DrawBackground(IImageProcessingContext x)
    {
        x.DrawImage(_background, new Point(0, 0), 1);
    }
    private void DrawOverlay(IImageProcessingContext x)
    {
        x.DrawImage(_overlay, new Point(0, 0), 1);
    }

    private void DrawArrow(IImageProcessingContext x)
    {
        x.DrawImage(_cachedImages[MoveIcons.MoveRangeArrow], new Point(14, 13), 1);
    }

    private const int __columnCount = 5;
    private const int __rowCount = 6;

    private static bool IsInDisplayRange(int row, int column)
    {
        return column > 0 && column < 4 && row < 5;
    }

    private void DrawRange(IImageProcessingContext x, MoveRange range)
    {
        for (int column = 0; column < __columnCount; column++)
        {
            for (int row = 0; row < __rowCount; row++)
            {
                if (IsInDisplayRange(row: row, column: column) && range.GetInRange(row: row, column: column))
                {
                    x.DrawImage(_cachedImages[MoveIcons.MoveRangeGlowOrange], GetPoint(row: row, column: column), 1);
                }
            }
        }
    }

    private void DrawMoveSideEffects(IImageProcessingContext x, Move move, MoveRange range)
    {
        for (int column = 0; column < __columnCount; column++)
        {
            for (int row = 0; row < __rowCount; row++)
            {
                if (IsInDisplayRange(row: row, column: column) && range.GetInRange(row: row, column: column))
                {
                    if (move.Movement == MoveMovementId.KnockbackTarget)
                    {
                        var knockbackRow = row - 1;
                        if (IsInDisplayRange(row: knockbackRow, column: column))
                        {
                            x.DrawImage(_cachedImages[MoveIcons.MoveRangeGlowRed], GetPoint(row: knockbackRow, column: column), 1);
                        }
                    }
                    else if (move.Movement == MoveMovementId.MoveUser)
                    {
                        int moveUserRow = 3;
                        int moveUserColumn = 2;
                        int diff = -1;
                        if (move.MovementFlags.HasFlag(MoveMovementFlags.DoubleMovementDistance))
                        {
                            diff--;
                        }
                        if (move.MovementFlags.HasFlag(MoveMovementFlags.InvertMovementDirection))
                        {
                            diff = -diff;
                        }
                        moveUserRow += diff;
                        if (!range.GetInRange(row: moveUserRow, column: moveUserColumn) && IsInDisplayRange(row: moveUserRow, column: moveUserColumn))
                        {
                            x.DrawImage(_cachedImages[MoveIcons.MoveRangeGlowBlue], GetPoint(row: moveUserRow, column: moveUserColumn), 1);
                        }
                    }
                }
            }
        }
    }

    private void DrawMovePowerStars(IImageProcessingContext context, int power)
    {
        int starCount = MiscUtil.PowerToStarCount(power);
        var x = 72;
        var y = 14;
        for (int i = 0; i < starCount; i++)
        {
            context.DrawImage(_cachedImages[MoveIcons.PowerStar], new Point(x, y), 1);
            x -= 8;
        }
    }

    private void DrawMoveType(IImageProcessingContext context, TypeId type)
    {
        if (type == TypeId.NoType)
        {
            return;
        }
        context.DrawImage(_typeImages[type], new Point(46, 2), 1);
    }

    public Image<Rgba32> GetMoveRangePreview(MoveRange range)
    {
        var baseImg = new Image<Rgba32>(_background.Width, _background.Height);

        baseImg.Mutate(x =>
        {
            DrawBackground(x);
            DrawRange(x, range);
            DrawArrow(x);
            DrawOverlay(x);
        });

        return baseImg;
    }

    public Image<Rgba32> GetMovePreview(Move move, MoveRange range, MovePreviewOptions options = MovePreviewOptions.All)
    {
        var baseImg = GetMoveRangePreview(range);

        baseImg.Mutate(x =>
        {
            DrawBackground(x);
            DrawRange(x, range);
            DrawMoveSideEffects(x, move, range);
            DrawArrow(x);
            if (options.HasFlag(MovePreviewOptions.DrawPowerStars))
            {
                DrawMovePowerStars(x, move.Power);
            }
            if (options.HasFlag(MovePreviewOptions.DrawType))
            {
                DrawMoveType(x, move.Type);
            }

            DrawOverlay(x);
        });

        return baseImg;
    }

    public Image<Rgba32> GetGimmickPreview(Gimmick gimmick, MoveRange range, MovePreviewOptions options = MovePreviewOptions.All)
    {
        var baseImg = GetMoveRangePreview(range);

        baseImg.Mutate(x =>
        {
            DrawBackground(x);
            DrawRange(x, range);
            DrawArrow(x);
            if (options.HasFlag(MovePreviewOptions.DrawPowerStars))
            {
                DrawMovePowerStars(x, gimmick.AttackPower);
            }
            if (options.HasFlag(MovePreviewOptions.DrawType))
            {
                DrawMoveType(x, gimmick.AttackType);
            }

            DrawOverlay(x);
        });

        return baseImg;
    }

    private static Point GetPoint(int row, int column)
    {
        return new Point(
            x: 18 + 6 * (column - row),
            y: -3 + 3 * (column + row)
            );
    }

    public void Dispose()
    {
        foreach (var image in _cachedImages.Values)
        {
            image.Dispose();
        }
        _cachedImages.Clear();
    }
}
