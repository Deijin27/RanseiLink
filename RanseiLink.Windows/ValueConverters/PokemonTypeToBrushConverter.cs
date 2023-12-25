using RanseiLink.Core.Enums;
using System.Windows.Media;

namespace RanseiLink.Windows.ValueConverters;

public class PokemonTypeToBrushConverter : ValueConverter<TypeId, Brush>
{
    private static readonly Dictionary<TypeId, Brush> _brushMap = new();
    static PokemonTypeToBrushConverter()
    {
        AddBrush(TypeId.Normal, "#EFE7E7");
        AddBrush(TypeId.Fire, "#F7736B");
        AddBrush(TypeId.Water, "#5AB5F7");
        AddBrush(TypeId.Electric, "#FFFF00");
        AddBrush(TypeId.Grass, "#73CE00");
        AddBrush(TypeId.Ice, "#ADF7F7");
        AddBrush(TypeId.Fighting, "#EFAD21");
        AddBrush(TypeId.Poison, "#BD63CE");
        AddBrush(TypeId.Ground, "#D6C652");
        AddBrush(TypeId.Flying, "#ADC6F7");
        AddBrush(TypeId.Psychic, "#FFADCE");
        AddBrush(TypeId.Bug, "#BDDE31");
        AddBrush(TypeId.Rock, "#B59C5A");
        AddBrush(TypeId.Ghost, "#5263A5");
        AddBrush(TypeId.Dragon, "#9473CE");
        AddBrush(TypeId.Dark, "#6B5A42");
        AddBrush(TypeId.Steel, "#ADADAD");
        AddBrush(TypeId.NoType, "#000000");
    }

    private static void AddBrush(TypeId terrain, string hexColor)
    {
        Color color = (Color)ColorConverter.ConvertFromString(hexColor);
        Brush brush = new SolidColorBrush(color);
        brush.Freeze();
        _brushMap.Add(terrain, brush);
    }


    protected override Brush Convert(TypeId value)
    {
        return _brushMap[value];
    }

    protected override TypeId ConvertBack(Brush value)
    {
        throw new System.NotImplementedException();
    }
}
