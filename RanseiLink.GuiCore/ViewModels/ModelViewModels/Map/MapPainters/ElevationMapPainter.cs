using RanseiLink.Core.Util;
using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.GuiCore.ViewModels;

public class ElevationMapPainter : BaseMapPainter
{
    public override string Name => "Elevation";


    private bool _paintEntireCell;

    public bool PaintEntireCell
    {
        get => _paintEntireCell;
        set => SetProperty(ref _paintEntireCell, value);
    }

    private static Rgba32 ZToColor(float value)
    {
        return ColorUtil.ColorFromHSV(
            hue: (double)value / 40 / 25 * 255, 
            saturation: 0.6, 
            value: 0.6
            );
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
            set
            {
                if (SetProperty(ref _value, value))
                {
                    RaisePropertyChanged(nameof(Color));
                    RaisePropertyChanged(nameof(NumericValue));
                    RaisePropertyChanged(nameof(IsValid));
                }
            }
        }

        public Brush(float value)
        {
            Value = value.ToString();
        }

        public bool IsValid => float.TryParse(Value, out var value) && !float.IsNaN(value) && !float.IsInfinity(value);

        public float NumericValue => float.TryParse(Value, out var value) ? value : 0;

        private static readonly Rgba32 __defaultColor = SixLabors.ImageSharp.Color.Black;
        public Rgba32 Color
        {
            get
            {
                if (!IsValid)
                {
                    return __defaultColor;
                }
                return ZToColor(NumericValue);
            }
        }
    }

    public ElevationMapPainter()
    {
        Brushes = [];
        Brushes.Add(new Brush(0f));
        Brushes.Add(new Brush(12.5f));
        Brushes.Add(new Brush(25f));
        Brushes.Add(new Brush(37.5f));
        Brushes.Add(new Brush(50f));
        for (int i = 0; i < 47; i++)
        {
            Brushes.Add(new Brush(0f));
        }
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
