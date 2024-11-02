using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.GuiCore.ViewModels;

public class ElevationMapPainter : BaseMapPainter
{
    private const double __value = 0.6;
    private const double __saturation = 0.6;

    public override string Name => "Elevation";

    private static Rgba32 ColorFromHSV(double hue, double saturation, double value)
    {
        int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        double f = hue / 60 - Math.Floor(hue / 60);

        value *= 255;
        byte v = (byte)value;
        byte p = (byte)(value * (1 - saturation));
        byte q = (byte)(value * (1 - f * saturation));
        byte t = (byte)(value * (1 - (1 - f) * saturation));

        return hi switch
        {
            0 => new Rgba32(v, t, p),
            1 => new Rgba32(q, v, p),
            2 => new Rgba32(p, v, t),
            3 => new Rgba32(p, q, v),
            4 => new Rgba32(t, p, v),
            _ => new Rgba32(v, p, q)
        };
    }

    private bool _paintEntireCell;

    public bool PaintEntireCell
    {
        get => _paintEntireCell;
        set => SetProperty(ref _paintEntireCell, value);
    }

    private static Rgba32 ZToColor(float value)
    {
        return ColorFromHSV((double)value / 40 / 25 * 255, __saturation, __value);
    }

    public override Rgba32 GetSubCellColor(MapGridSubCellViewModel subCell)
    {
        return ZToColor(subCell.Z);
    }

    public List<Brush> Brushes { get; }

    private Brush _selectedBrush;
    public Brush SelectedBrush
    {
        get => _selectedBrush;
        set => SetProperty(ref _selectedBrush, value);
    }

    public class Brush : ViewModelBase
    {
        private string _value = "12.5";
        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public Brush(float value)
        {
            Value = value.ToString();
        }

        public bool IsValid => float.TryParse(Value, out var value) && !float.IsNaN(value) && !float.IsInfinity(value);

        public float NumericValue => float.TryParse(Value, out var value) ? value : 0;

        public Rgba32 Color => ZToColor(NumericValue);
    }

    public ElevationMapPainter()
    {
        Brushes = [];
        Brushes.Add(new Brush(12.5f));
        _selectedBrush = Brushes[0];
    }

    public override void OnMouseDownOnCell(MapGridCellViewModel cell)
    {
        if (_paintEntireCell)
        {
            var elevationToPaint = SelectedBrush.NumericValue;
            foreach (var subCell in cell.SubCells)
            {
                subCell.Z = elevationToPaint;
            }
        }
    }

    public override void OnMouseDownOnSubCell(MapGridSubCellViewModel cell)
    {
        if (!_paintEntireCell)
        {
            cell.Z = SelectedBrush.NumericValue;
        }
    }
}
