using RanseiLink.Core;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.GuiCore.ViewModels;

public class BoundsMapPainter : BaseMapPainter
{
    private readonly Dictionary<MapBounds, Rgba32> _boundsToColor = [];

    public override string Name => "Bounds";

    private Brush _selectedBrush;

    public record Brush(MapBounds Id, string Name, Rgba32 Color);

    public List<Brush> Brushes { get; }

    public Brush SelectedBrush
    {
        get => _selectedBrush;
        set => SetProperty(ref _selectedBrush, value);
    }

    public Rgba32 HueToColor(double hue)
    {
        return ColorUtil.ColorFromHSV(
            hue: hue,
            saturation: 0.6,
            value: 0.6
            );
    }

    public BoundsMapPainter()
    {
        var scale = 15;
        AddBrush(MapBounds.Pokemon_0, 0 * scale);
        AddBrush(MapBounds.Pokemon_1, 1 * scale);
        AddBrush(MapBounds.Pokemon_2, 2 * scale);
        AddBrush(MapBounds.Pokemon_3, 3 * scale);
        AddBrush(MapBounds.Pokemon_4, 4 * scale);
        AddBrush(MapBounds.Pokemon_5, 5 * scale);
        AddBrush(MapBounds.Pokemon_6, 6 * scale);
        AddBrush(MapBounds.Pokemon_7, 7 * scale);
        AddBrush(MapBounds.Pokemon_8, 8 * scale);
        AddBrush(MapBounds.Pokemon_9, 9 * scale);
        AddBrush(MapBounds.Pokemon_10, 10 * scale);
        AddBrush(MapBounds.Pokemon_11, 11 * scale);
        AddBrush(MapBounds.Unknown_0, 320 - 0 * scale);
        AddBrush(MapBounds.Unknown_1, 320 - 1 * scale);
        AddBrush(MapBounds.OutOfBounds, "#1E2021");
        AddBrush(MapBounds.InBounds, "#DAD3C5");

        Brushes = [];
        foreach (var terrain in EnumUtil.GetValues<MapBounds>())
        {
            Brushes.Add(new Brush(
                Id: terrain,
                Name: terrain.ToString(),
                Color: _boundsToColor.TryGetValue(terrain, out var color) ? color : Color.IndianRed
                ));
        }
        _defaultColor = HueToColor(320 - 2 * scale);
        _selectedBrush = Brushes.First(x => x.Id == MapBounds.InBounds);
    }

    private Rgba32 _defaultColor;

    public void AddBrush(MapBounds id, string hex)
    {
        _boundsToColor[id] = Rgba32.ParseHex(hex);
    }

    public void AddBrush(MapBounds id, double hue)
    {
        _boundsToColor[id] = HueToColor(hue);
    }

    public override Rgba32 GetCellColor(MapGridCellViewModel cell)
    {
        if (_boundsToColor.TryGetValue(cell.Bounds, out var color))
        {
            return color;
        }
        return _defaultColor;
    }

    public override void OnMouseDownOnCell(MapGridCellViewModel cell)
    {
        cell.Bounds = _selectedBrush.Id;
    }
}
