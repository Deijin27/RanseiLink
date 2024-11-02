using RanseiLink.Core;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Services;
using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.GuiCore.ViewModels;

public class TerrainMapPainter : BaseMapPainter
{
    public override string Name => "Terrain";

    private readonly Dictionary<TerrainId, Rgba32> _terrainToColor = [];
    private readonly IPathToImageConverter _pathToImageConverter;
    private readonly IOverrideDataProvider _overrideDataProvider;
    private Brush _selectedBrush;

    public record Brush(TerrainId Id, string Name, Rgba32 Color, object? Image);

    public List<Brush> Brushes { get; }

    public Brush SelectedBrush
    {
        get => _selectedBrush;
        set => SetProperty(ref _selectedBrush, value);
    }

    private string TerrainBrushImagePath(TerrainId id)
    {
        return _overrideDataProvider.GetSpriteFile(SpriteType.StlChikei, (int)id).File;
    }

    public TerrainMapPainter(IPathToImageConverter pathToImageConverter, IOverrideDataProvider overrideDataProvider)
    {
        _pathToImageConverter = pathToImageConverter;
        _overrideDataProvider = overrideDataProvider;

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

        Brushes = [];
        foreach (var terrain in EnumUtil.GetValues<TerrainId>())
        {
            Brushes.Add(new Brush(
                Id: terrain, 
                Name: terrain.ToString(),
                Color: _terrainToColor[terrain],
                Image: _pathToImageConverter.TryConvert(TerrainBrushImagePath(terrain))
                ));
        }
        _selectedBrush = Brushes.First(x => x.Id == TerrainId.Default);
        
    }

    public void AddBrush(TerrainId id, string hex)
    {
        _terrainToColor[id] = Rgba32.ParseHex(hex);
    }

    public override Rgba32 GetCellColor(MapGridCellViewModel cell)
    {
        return _terrainToColor[cell.Terrain];
    }

    public override void OnMouseDownOnCell(MapGridCellViewModel cell)
    {
        cell.Terrain = _selectedBrush.Id;
    }

}
