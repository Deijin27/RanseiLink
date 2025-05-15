using RanseiLink.Core.Maps;
using System.Windows.Media;

namespace RanseiLink.Windows.ValueConverters;

public class TerrainToBrushConverter : ValueConverter<TerrainId, Brush>
{
    private static void AddBrush(TerrainId terrain, string hexColor)
    {
        Color color = (Color)ColorConverter.ConvertFromString(hexColor);
        Brush brush = new SolidColorBrush(color);
        brush.Freeze();
        __brushMap.Add(terrain, brush);
    }

    static TerrainToBrushConverter()
    {
        __brushMap = [];
        foreach (var color in TerrainToColorConverter.TerrainToHex)
        {
            AddBrush(color.Key, color.Value);
        }
    }

    private static readonly Dictionary<TerrainId, Brush> __brushMap;

    protected override Brush Convert(TerrainId value)
    {
        return __brushMap[value];
    }

    protected override TerrainId ConvertBack(Brush value)
    {
        throw new NotImplementedException();
    }
}
