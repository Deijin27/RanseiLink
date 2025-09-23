using RanseiLink.Core.Settings;
using RanseiLink.Core.Util;
using RanseiLink.GuiCore.Constants;
using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.GuiCore.ViewModels;

public class ElevationMapPainter : BaseMapPainter
{
    public override string Name => "Elevation";
    public override IconId Icon => IconId.elevation;

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
        private string _value;
        
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
                    ValueChanged?.Invoke();
                }
            }
        }

        public event Action? ValueChanged;

        public Brush(float value)
        {
            _value = value.ToString();
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

    public class CellMode
    {
        public static readonly CellMode Cell = new("3 × 3", IconId.grid_on);
        public static readonly CellMode SubCell = new("1 × 1", IconId.stop);
        public static readonly CellMode Picker = new("Picker", IconId.colorize);

        private CellMode(string name, IconId id)
        {
            Name = name;
            Icon = id;
        }

        public string Name { get; }
        public IconId Icon { get; }
    }

    public CellMode[] Modes { get; } = [CellMode.Cell, CellMode.SubCell, CellMode.Picker];

    private static CellMode __selectedMode = CellMode.SubCell;
    private readonly ISettingService _settingService;
    private ElevationPaletteSetting _elevationPaletteSetting;

    public CellMode SelectedMode
    {
        get => __selectedMode;
        set => SetProperty(ref __selectedMode, value);
    }

    public ElevationMapPainter(ISettingService settingService)
    {
        _settingService = settingService;
        _elevationPaletteSetting = settingService.Get<ElevationPaletteSetting>();
        Brushes = [];
        Brushes.Add(new Brush(0f));
        Brushes.Add(new Brush(12.5f));
        Brushes.Add(new Brush(25f));
        Brushes.Add(new Brush(37.5f));
        Brushes.Add(new Brush(50f));
        for (int i = 0; i < 103; i++)
        {
            Brushes.Add(new Brush(0f));
        }
        _selectedBrush = Brushes[0];

        int c = 0;
        foreach (var x in _elevationPaletteSetting.Value)
        {
            if (c >= Brushes.Count)
            {
                break;
            }
            Brushes[c].Value = x.ToString();
            c++;
        }


        foreach (var brush in Brushes)
        {
            brush.ValueChanged += Brush_ValueChanged;
        }
    }

    private void Brush_ValueChanged()
    {
        _elevationPaletteSetting.Value = Brushes.Select(x => x.Value).Select(x => float.TryParse(x, out var xParsed) ? xParsed : 0f).ToArray();
        _settingService.Save();
    }

    public override void OnMouseDownOnCell(MapGridCellViewModel cell)
    {
        if (SelectedMode == CellMode.Cell)
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
        if (SelectedMode == CellMode.SubCell)
        {
            cell.Z = SelectedBrush.NumericValue;
        }
        else if (SelectedMode == CellMode.Picker)
        {
            SelectedBrush.Value = cell.Z.ToString();
        }
    }
}
