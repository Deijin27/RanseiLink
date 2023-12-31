using RanseiLink.Windows.ViewModels;
using System.Windows.Media;

namespace RanseiLink.Windows.ValueConverters;

public class SubCellToBrushConverter : ValueConverter<MapSubCellInfo, Brush>
{
    private static readonly ZToHueConverter _zToHue = new();
    protected override Brush Convert(MapSubCellInfo value)
    {
        return ConvertValue(value);
    }

    protected override MapSubCellInfo ConvertBack(Brush value)
    {
        throw new NotImplementedException();
    }

    public static Brush ConvertValue(MapSubCellInfo value)
    {
        return value.RenderMode switch
        {
            MapRenderMode.Terrain => Brushes.Transparent,
            MapRenderMode.Elevation => _zToHue.ConvertValue(value.Z),
            _ => throw new Exception($"Invalid {nameof(MapRenderMode)}"),
        };
    }
}
