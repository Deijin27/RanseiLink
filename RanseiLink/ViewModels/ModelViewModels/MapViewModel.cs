using RanseiLink.Core.Enums;
using RanseiLink.Core.Map;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public delegate MapViewModel MapViewModelFactory(Map model);

public class MapGridCellViewModel : ViewModelBase
{
    private MapTerrainEntry _terrainEntry;
    private GimmickId? _containsGimmick = null;
    private bool _containsPokemon = false;

    public MapGridCellViewModel(MapTerrainEntry entry, uint x, uint y)
    {
        _terrainEntry = entry;

        X = x;
        Y = y;
    }

    public uint X { get; }
    public uint Y { get; }

    public Terrain Terrain
    {
        get => _terrainEntry.Terrain;
        set => RaiseAndSetIfChanged(_terrainEntry.Terrain, value, v => _terrainEntry.Terrain = v);
    }

    public GimmickId? ContainsGimmick
    {
        get => _containsGimmick;
        set => RaiseAndSetIfChanged(ref _containsGimmick, value);
    }
    public bool ContainsPokemon
    {
        get => _containsPokemon;
        set => RaiseAndSetIfChanged(ref _containsPokemon, value);
    }
}

public class MapViewModel : ViewModelBase
{
    private MapGridCellViewModel _mouseOverItem;

    public Map Map { get; set; }

    public MapViewModel(Map model)
    {
        Map = model;
        Matrix = new();
        uint y = 0;
        foreach (var row in Map.TerrainSection.MapMatrix)
        {
            uint x = 0;
            var rowItems = new List<MapGridCellViewModel>();
            foreach (var col in row)
            {
                rowItems.Add(new MapGridCellViewModel(col, x++, y));
            }
            Matrix.Add(rowItems);
            y++;
        }

        foreach (var pokemonPos in Map.PositionSection.Positions)
        {
            Matrix[pokemonPos.Y][pokemonPos.X].ContainsPokemon = true;
        }

        foreach (var gimmick in Map.GimmickSection.Items)
        {
            Matrix[gimmick.Position.Y][gimmick.Position.X].ContainsGimmick = gimmick.Gimmick;
        }
    }

    public uint Width
    {
        get => Map.Header.Width;
        set => RaiseAndSetIfChanged(Map.Header.Width, value, v => Map.Header.Width = (ushort)v);
    }

    public uint Height
    {
        get => Map.Header.Height;
        set => RaiseAndSetIfChanged(Map.Header.Height, value, v => Map.Header.Height = (ushort)v);
    }

    public ObservableCollection<List<MapGridCellViewModel>> Matrix { get; }

    public MapGridCellViewModel MouseOverItem
    {
        get => _mouseOverItem;
        set => RaiseAndSetIfChanged(ref _mouseOverItem, value);
    }
}
