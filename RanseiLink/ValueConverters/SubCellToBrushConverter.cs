using RanseiLink.ViewModels;
using System;
using System.Windows.Media;

namespace RanseiLink.ValueConverters;

public class SubCellToBrushConverter : ValueConverter<MapGridSubCellViewModel, Brush>
{
    private static readonly ZToHueConverter _zToHue = new();
    protected override Brush Convert(MapGridSubCellViewModel value)
    {
        return value.RenderMode switch
        {
            MapRenderMode.Terrain => Brushes.Transparent,
            MapRenderMode.Elevation => _zToHue.ConvertValue(value.Z),
            _ => throw new Exception($"Invalid {nameof(MapRenderMode)}"),
        };
    }

    protected override MapGridSubCellViewModel ConvertBack(Brush value)
    {
        throw new NotImplementedException();
    }
}
