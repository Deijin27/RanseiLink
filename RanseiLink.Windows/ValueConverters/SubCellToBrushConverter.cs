using RanseiLink.Windows.ViewModels;
using System.Windows.Media;

namespace RanseiLink.Windows.ValueConverters;

public class SubCellToBrushConverter : ValueConverter<MapGridSubCellViewModel, Brush>
{
    private static readonly ZToHueConverter _zToHue = new();
    protected override Brush Convert(MapGridSubCellViewModel value)
    {
        return ConvertValue(value);
    }

    protected override MapGridSubCellViewModel ConvertBack(Brush value)
    {
        throw new NotImplementedException();
    }

    public static Brush ConvertValue(MapGridSubCellViewModel value)
    {
        return value.RenderMode switch
        {
            MapRenderMode.Terrain => Brushes.Transparent,
            MapRenderMode.Elevation => _zToHue.ConvertValue(value.Z),
            _ => throw new Exception($"Invalid {nameof(MapRenderMode)}"),
        };
    }
}
