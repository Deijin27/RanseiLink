using RanseiLink.Core.Map;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace RanseiLink.ValueConverters;

public class TerrainToBrushConverter : ValueConverter<Terrain, Brush>
{
    private static void AddBrush(Terrain terrain, string hexColor)
    {
        Color color = (Color)ColorConverter.ConvertFromString(hexColor);
        Brush brush = new SolidColorBrush(color);
        brush.Freeze();
        _brushMap.Add(terrain, brush);
    }

    static TerrainToBrushConverter()
    {
        _brushMap = new();
        AddBrush(Terrain.Normal, "#FFFFFF");
        AddBrush(Terrain.Magma, "#BE0A0F");
        AddBrush(Terrain.Water, "#33B7B7");
        AddBrush(Terrain.Electric, "#FCF45D");
        AddBrush(Terrain.Grass, "#56BC2D");
        AddBrush(Terrain.Ice, "#ACD0E6");
        AddBrush(Terrain.Fighting, "#723C3A");
        AddBrush(Terrain.Poison, "#6F0080");
        AddBrush(Terrain.Ground, "#E0C068");
        AddBrush(Terrain.Flying, "#A890F0");
        AddBrush(Terrain.Psychic, "#F85888");
        AddBrush(Terrain.Bug, "#A8B820");
        AddBrush(Terrain.Rock, "#938570");
        AddBrush(Terrain.Ghost, "#705898");
        AddBrush(Terrain.Dragon, "#7038F8");
        AddBrush(Terrain.Dark, "#705848");
        AddBrush(Terrain.Steel, "#919A92");
        AddBrush(Terrain.Sand, "#ECE1B1");
        AddBrush(Terrain.Soil, "#62361F");
        AddBrush(Terrain.Snow, "#DBE2E9");
        AddBrush(Terrain.Swamp, "#646032");
        AddBrush(Terrain.Bog, "#4E3F26");
        AddBrush(Terrain.Scaffolding, "#A18B3C");
        AddBrush(Terrain.Hidden, "#B0783F");
        AddBrush(Terrain.HiddenPoison, "#701F4D");
        AddBrush(Terrain.Unused_1, "#CF4CC9");
        AddBrush(Terrain.Void, "#1E2021");
        AddBrush(Terrain.Default, "#DAD3C5");
    }

    private static readonly Dictionary<Terrain, Brush> _brushMap;

    protected override Brush Convert(Terrain value)
    {
        return _brushMap[value];
    }

    protected override Terrain ConvertBack(Brush value)
    {
        throw new NotImplementedException();
    }
}
