using Avalonia.Media;
using RanseiLink.Core.Enums;
using Color = Avalonia.Media.Color;

namespace RanseiLink.XP.ValueConverters;

public class PokemonTypeToBrushConverter : ValueConverter<TypeId, Brush>
{
    public static PokemonTypeToBrushConverter Instance = new();
    
    private static readonly Dictionary<TypeId, Brush> __brushMap = new();
    static PokemonTypeToBrushConverter()
    {
        foreach (var (type, hex) in PokemonTypeToColorConverter.TypeToHex)
        {
            AddBrush(type, hex);
        }
    }

    private static void AddBrush(TypeId terrain, string hexColor)
    {
        Color color = Color.Parse(hexColor);
        Brush brush = new SolidColorBrush(color);
        __brushMap.Add(terrain, brush);
    }


    protected override Brush Convert(TypeId value)
    {
        return __brushMap[value];
    }

    protected override TypeId ConvertBack(Brush value)
    {
        throw new System.NotSupportedException();
    }
}