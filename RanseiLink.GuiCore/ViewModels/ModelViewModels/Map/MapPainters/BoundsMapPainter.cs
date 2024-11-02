using RanseiLink.Core;
using RanseiLink.Core.Maps;
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

    public BoundsMapPainter()
    {
        AddBrush(MapBounds.Pokemon_0, "#FFFFFF");
        AddBrush(MapBounds.Pokemon_1, "#FFFFFF");
        AddBrush(MapBounds.Pokemon_2, "#FFFFFF");
        AddBrush(MapBounds.Pokemon_3, "#FFFFFF");
        AddBrush(MapBounds.Pokemon_4, "#FFFFFF");
        AddBrush(MapBounds.Pokemon_5, "#FFFFFF");
        AddBrush(MapBounds.Pokemon_6, "#FFFFFF");
        AddBrush(MapBounds.Pokemon_7, "#FFFFFF");
        AddBrush(MapBounds.Pokemon_8, "#FFFFFF");
        AddBrush(MapBounds.Pokemon_9, "#FFFFFF");
        AddBrush(MapBounds.Pokemon_10, "#FFFFFF");
        AddBrush(MapBounds.Pokemon_11, "#FFFFFF");
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
        _selectedBrush = Brushes.First(x => x.Id == MapBounds.InBounds);
    }

    public void AddBrush(MapBounds id, string hex)
    {
        _boundsToColor[id] = Rgba32.ParseHex(hex);
    }

    public override Rgba32 GetCellColor(MapGridCellViewModel cell)
    {
        if (_boundsToColor.TryGetValue(cell.Bounds, out var color))
        {
            return color;
        }
        return Color.IndianRed;
    }

    public override void OnMouseDownOnCell(MapGridCellViewModel cell)
    {
        cell.Bounds = _selectedBrush.Id;
    }
}
