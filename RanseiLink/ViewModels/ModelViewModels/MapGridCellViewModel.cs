using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RanseiLink.ViewModels;


public class MapGridCellViewModel : ViewModelBase
{
    public MapTerrainEntry TerrainEntry { get; }

    private bool _isSelected;

    private readonly IGimmickService _gimmickService;
    private readonly IOverrideDataProvider _spriteProvider;
    public MapGridCellViewModel(MapTerrainEntry entry, int x, int y, MapRenderMode renderMode, IGimmickService gimmickService, IOverrideDataProvider spriteProvider)
    {
        _gimmickService = gimmickService;
        _spriteProvider = spriteProvider;
        TerrainEntry = entry;

        X = x;
        Y = y;

        SubCell0 = new(this, 0, renderMode);
        SubCell1 = new(this, 1, renderMode);
        SubCell2 = new(this, 2, renderMode);
        SubCell3 = new(this, 3, renderMode);
        SubCell4 = new(this, 4, renderMode);
        SubCell5 = new(this, 5, renderMode);
        SubCell6 = new(this, 6, renderMode);
        SubCell7 = new(this, 7, renderMode);
        SubCell8 = new(this, 8, renderMode);
    }

    public void AddGimmick(MapGimmickViewModel gimmickViewModel)
    {
        Gimmicks.Add(gimmickViewModel);
        gimmickViewModel.PropertyChanged += OnGimmickVmGimmickChanged;
        UpdateVisibleGimmick();
    }

    public void RemoveGimmick(MapGimmickViewModel gimmickViewModel)
    {
        Gimmicks.Remove(gimmickViewModel);
        gimmickViewModel.PropertyChanged -= OnGimmickVmGimmickChanged;
        UpdateVisibleGimmick();
    }

    private void OnGimmickVmGimmickChanged(object sender, EventArgs e)
    {
        UpdateVisibleGimmick();
    }

    private void UpdateVisibleGimmick()
    {
        if (Gimmicks.Any())
        {
            var gimmick = Gimmicks.Last();
            GimmickImagePath = _spriteProvider.GetSpriteFile(SpriteType.StlStageObje, _gimmickService.Retrieve((int)gimmick.Gimmick).Image).File;
        }
        else
        {
            GimmickImagePath = null;
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => RaiseAndSetIfChanged(ref _isSelected, value);
    }

    public int X { get; }
    public int Y { get; }

    public TerrainId Terrain
    {
        get => TerrainEntry.Terrain;
        set => RaiseAndSetIfChanged(TerrainEntry.Terrain, value, v => TerrainEntry.Terrain = v);
    }

    public string Unknown3
    {
        get => TerrainEntry.Unknown3 switch
        {
            0 => "In Bounds",
            1 => "Out Of Bounds",
            > 1 and < 14 => $"Pokemon {TerrainEntry.Unknown3 - 2}",
            _ => $"{TerrainEntry.Unknown3}"
        };
    }

    public int Unknown3Numeric
    {
        get => TerrainEntry.Unknown3;
        set => RaiseAndSetIfChanged(TerrainEntry.Unknown3, (byte)value, v => TerrainEntry.Unknown3 = v);
    }

    public int Unknown4
    {
        get => TerrainEntry.Unknown4;
        set => RaiseAndSetIfChanged(TerrainEntry.Unknown4, (byte)value, v => TerrainEntry.Unknown4 = v);
    }

    public int Unknown5
    {
        get => TerrainEntry.Unknown5;
        set => RaiseAndSetIfChanged(TerrainEntry.Unknown5, (byte)value, v => TerrainEntry.Unknown5 = v);
    }

    public ObservableCollection<MapGimmickViewModel> Gimmicks { get; } = new();

    public ObservableCollection<MapPokemonPositionViewModel> Pokemon { get; } = new();

    public string GimmicksString => string.Join(", ", Gimmicks.Select(i => i.Gimmick));

    private string _gimmickImagePath;
    public string GimmickImagePath
    {
        get => _gimmickImagePath;
        set => RaiseAndSetIfChanged(ref _gimmickImagePath, value);
    }

    public MapGridSubCellViewModel SubCell0 { get; }
    public MapGridSubCellViewModel SubCell1 { get; }
    public MapGridSubCellViewModel SubCell2 { get; }
    public MapGridSubCellViewModel SubCell3 { get; }
    public MapGridSubCellViewModel SubCell4 { get; }
    public MapGridSubCellViewModel SubCell5 { get; }
    public MapGridSubCellViewModel SubCell6 { get; }
    public MapGridSubCellViewModel SubCell7 { get; }
    public MapGridSubCellViewModel SubCell8 { get; }

}

