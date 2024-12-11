using RanseiLink.Core.Graphics;
using RanseiLink.Core.Maps;
using RanseiLink.GuiCore.Constants;
using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.GuiCore.ViewModels;

public class OrientationMapPainter : BaseMapPainter
{
    public override string Name => "Orientation";
    public override IconId Icon => IconId.explore;

    private readonly Dictionary<OrientationAlt, Brush> _orientationToColor = [];
    private Brush _selectedBrush;

    public record Brush(OrientationAlt Id, string Name, Rgba32 ColorBg, Rgba32 ColorFg, int Pattern);

    public List<Brush> Brushes { get; } = [];

    public Brush SelectedBrush
    {
        get => _selectedBrush;
        set => SetProperty(ref _selectedBrush, value);
    }

    public OrientationMapPainter()
    {
        AddBrush(OrientationAlt.North, "#BE0A0F", "#591c1e", 0b_010_101_000);
        AddBrush(OrientationAlt.East, "#FCF45D", "#8c7335", 0b_010_001_010);
        AddBrush(OrientationAlt.South, "#56BC2D", "#36512b", 0b_000_101_010);
        AddBrush(OrientationAlt.West, "#33B7B7", "#275360", 0b_010_100_010);
        AddBrush(OrientationAlt.Unknown_4, "#6F0080", "#6F0080", 0b_000_000_000);
        AddBrush(OrientationAlt.Unknown_7, "#F85888", "#F85888", 0b_000_000_000);
        AddBrush(OrientationAlt.Unknown_5, "#DAD3C5", "#DAD3C5", 0b_000_000_000);

        _selectedBrush = Brushes.First(x => x.Id == OrientationAlt.West);
    }

    public void AddBrush(OrientationAlt id, string hexBg, string hexFg, int pattern)
    {
        var brush = new Brush(
                Id: id,
                Name: id.ToString(),
                ColorBg: Rgba32.ParseHex(hexBg),
                ColorFg: Rgba32.ParseHex(hexFg),
                Pattern: pattern
                );
        _orientationToColor[id] = brush;
        Brushes.Add(brush);
    }

    public override Rgba32 GetSubCellColor(MapGridSubCellViewModel subCell)
    {
        if (_orientationToColor.TryGetValue(subCell.Parent.Orientation, out var color))
        {
            var isForeground =((color.Pattern >> (8 - subCell.VisualId)) & 1) == 1;
            if (isForeground)
            {
                return color.ColorFg;
            }
            else
            {
                return color.ColorBg;
            }
        }
        else
        {
            return _orientationToColor[OrientationAlt.Unknown_4].ColorBg;
        }
    }

    public override void OnMouseDownOnCell(MapGridCellViewModel cell)
    {
        cell.Orientation = _selectedBrush.Id;
    }

}
