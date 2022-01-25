using RanseiLink.Core.Maps;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace RanseiLink.ValueConverters;

public class TerrainToBrushConverter : ValueConverter<TerrainId, Brush>
{
    private static void AddBrush(TerrainId terrain, string hexColor)
    {
        Color color = (Color)ColorConverter.ConvertFromString(hexColor);
        Brush brush = new SolidColorBrush(color);
        brush.Freeze();
        _brushMap.Add(terrain, brush);
    }

    static TerrainToBrushConverter()
    {
        _brushMap = new();
        AddBrush(TerrainId.Normal, "#FFFFFF");
        AddBrush(TerrainId.Magma, "#BE0A0F");
        AddBrush(TerrainId.Water, "#33B7B7");
        AddBrush(TerrainId.Electric, "#FCF45D");
        AddBrush(TerrainId.Grass, "#56BC2D");
        AddBrush(TerrainId.Ice, "#ACD0E6");
        AddBrush(TerrainId.Fighting, "#723C3A");
        AddBrush(TerrainId.Poison, "#6F0080");
        AddBrush(TerrainId.Ground, "#E0C068");
        AddBrush(TerrainId.Flying, "#A890F0");
        AddBrush(TerrainId.Psychic, "#F85888");
        AddBrush(TerrainId.Bug, "#A8B820");
        AddBrush(TerrainId.Rock, "#938570");
        AddBrush(TerrainId.Ghost, "#705898");
        AddBrush(TerrainId.Dragon, "#7038F8");
        AddBrush(TerrainId.Dark, "#705848");
        AddBrush(TerrainId.Steel, "#919A92");
        AddBrush(TerrainId.Sand, "#ECE1B1");
        AddBrush(TerrainId.Soil, "#62361F");
        AddBrush(TerrainId.Snow, "#DBE2E9");
        AddBrush(TerrainId.Swamp, "#646032");
        AddBrush(TerrainId.Bog, "#4E3F26");
        AddBrush(TerrainId.Scaffolding, "#A18B3C");
        AddBrush(TerrainId.Hidden, "#B0783F");
        AddBrush(TerrainId.HiddenPoison, "#701F4D");
        AddBrush(TerrainId.Unused_1, "#CF4CC9");
        AddBrush(TerrainId.Void, "#1E2021");
        AddBrush(TerrainId.Default, "#DAD3C5");
    }

    private static readonly Dictionary<TerrainId, Brush> _brushMap;

    protected override Brush Convert(TerrainId value)
    {
        return _brushMap[value];
    }

    protected override TerrainId ConvertBack(Brush value)
    {
        throw new NotImplementedException();
    }
}
