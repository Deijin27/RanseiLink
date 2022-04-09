using RanseiLink.Core.Maps;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public enum MapRenderMode
{
    Terrain,
    Elevation
}

public interface IMapViewModel
{
    void SetModel(PSLM model);
}

public class MapViewModel : ViewModelBase, IMapViewModel
{
    private static bool _terrainPaintingActive;
    private static TerrainId _terrainBrush;
    private static MapRenderMode _mapRenderMode; // static so it's preserved between pages
    private MapGimmickViewModel _selectedGimmick;
    private MapPokemonPositionViewModel _selectedPokemonPosition;
    private MapGridSubCellViewModel _mouseOverItem;
    private MapGridCellViewModel _selectedCell;
    private readonly IDialogService _dialogService;
    private readonly IGimmickService _gimmickService;
    private readonly IOverrideSpriteProvider _spriteProvider;

    public PSLM Map { get; set; }

    public MapViewModel(IDialogService dialogService, IGimmickService gimmickService, IOverrideSpriteProvider overrideSpriteProvider)
    {
        _dialogService = dialogService;
        _gimmickService = gimmickService;
        _spriteProvider = overrideSpriteProvider;
        
        RemoveSelectedGimmickCommand = new RelayCommand(RemoveSelectedGimmick, () => _selectedGimmick != null);
        ModifyMapDimensionsCommand = new RelayCommand(ModifyMapDimensions);
    }

    public void SetModel(PSLM model)
    {
        Map = model;
        Gimmicks.Clear();
        PokemonPositions.Clear();
        foreach (var gimmick in Map.GimmickSection.Items.Select(i => new MapGimmickViewModel(this, i)))
        {
            Gimmicks.Add(gimmick);
        }
        for (int i = 0; i < Map.PositionSection.Positions.Length; i++)
        {
            PokemonPositions.Add(new MapPokemonPositionViewModel(this, Map.PositionSection.Positions, i));
        }
        Draw();
    }

    public ICommand RemoveSelectedGimmickCommand { get; }
    public ICommand ModifyMapDimensionsCommand { get; }
    public ObservableCollection<MapGimmickViewModel> Gimmicks { get; } = new();
    public ObservableCollection<MapPokemonPositionViewModel> PokemonPositions { get; } = new();

    public TerrainId TerrainBrush
    {
        get => _terrainBrush;
        set 
        { 
            if (RaiseAndSetIfChanged(ref _terrainBrush, value)) 
            {
                RaisePropertyChanged(nameof(TerrainBrushImagePath));
            } 
        }
    }

    public string TerrainBrushImagePath => _spriteProvider.GetSpriteFile(SpriteType.StlChikei, (int)TerrainBrush).File;
    public bool TerrainPaintingActive
    {
        get => _terrainPaintingActive;
        set => RaiseAndSetIfChanged(ref _terrainPaintingActive, value);
    }

    public MapRenderMode RenderMode
    {
        get => _mapRenderMode;
        set
        {
            if (RaiseAndSetIfChanged(ref _mapRenderMode, value))
            {
                Draw();
            }
        }
    }

    public ushort Width => (ushort)Map.TerrainSection.MapMatrix[0].Count;

    public ushort Height => (ushort)Map.TerrainSection.MapMatrix.Count;

    public ObservableCollection<List<MapGridCellViewModel>> Matrix { get; } = new();

    public MapGridSubCellViewModel MouseOverItem
    {
        get => _mouseOverItem;
        set => RaiseAndSetIfChanged(ref _mouseOverItem, value);
    }

    public MapGimmickViewModel SelectedGimmick
    {
        get => _selectedGimmick;
        set
        {
            if (RaiseAndSetIfChanged(ref _selectedGimmick, value) && value != null)
            {
                SelectedCell = Matrix[value.Y][value.X];
            };
        }
    }

    public MapPokemonPositionViewModel SelectedPokemonPosition
    {
        get => _selectedPokemonPosition;
        set
        {
            if (RaiseAndSetIfChanged(ref _selectedPokemonPosition, value) && value != null)
            {
                SelectedCell = Matrix[value.Y][value.X];
            }
        }
    }

    public MapGridCellViewModel SelectedCell
    {
        get => _selectedCell;
        set
        {
            if (value != _selectedCell)
            {
                if (_selectedCell != null)
                {
                    _selectedCell.IsSelected = false;
                }
                _selectedCell = value;
                if (_selectedCell != null)
                {
                    _selectedCell.IsSelected = true;
                }
                RaisePropertyChanged();
            }
        }
    }

    public void Draw()
    {
        var selectedTerrainEntry = SelectedCell?.TerrainEntry ?? Map.TerrainSection.MapMatrix[0][0];
        Matrix.Clear();
        int y = 0;
        foreach (var row in Map.TerrainSection.MapMatrix)
        {
            int x = 0;
            var rowItems = new List<MapGridCellViewModel>();
            foreach (var col in row)
            {
                var cellVm = new MapGridCellViewModel(col, x++, y, RenderMode, _gimmickService, _spriteProvider);
                rowItems.Add(cellVm);
                if (col == selectedTerrainEntry)
                {
                    SelectedCell = cellVm;
                }
            }
            Matrix.Add(rowItems);
            y++;
        }

        foreach (var pokemon in PokemonPositions)
        {
            Matrix[pokemon.Y][pokemon.X].Pokemon.Add(pokemon);
        }

        foreach (var gimmick in Gimmicks)
        {
            Matrix[gimmick.Y][gimmick.X].Gimmicks.Add(gimmick);
        }
    }

    private bool GetInRange(Position position)
    {
        return position.X < Width && position.Y < Height;
    }

    public MapGimmickViewModel AddGimmick()
    {
        var mapGimmickItem = new MapGimmickItem();
        Map.GimmickSection.Items.Add(mapGimmickItem);
        var newGimmick = new MapGimmickViewModel(this, mapGimmickItem);
        Gimmicks.Add(newGimmick);
        SelectedGimmick = newGimmick;
        return newGimmick;
    }

    public void OnSubCellClicked(MapGridSubCellViewModel clickedSubCell)
    {
        SelectedCell = clickedSubCell.Parent;

        if (TerrainPaintingActive)
        {
            SelectedCell.Terrain = TerrainBrush;
        }

        _selectedGimmick = SelectedCell.Gimmicks.FirstOrDefault();
        RaisePropertyChanged(nameof(SelectedGimmick));
    }

    private void RemoveSelectedGimmick()
    {
        if (_selectedGimmick != null)
        {
            Map.GimmickSection.Items.Remove(_selectedGimmick.GimmickItem);
            Gimmicks.Remove(_selectedGimmick);
            _selectedGimmick = null;
        }
    }

    private void ModifyMapDimensions()
    {
        var width = Width;
        var height = Height;
        if (!_dialogService.ModifyMapDimensions(ref width, ref height))
        {
            return;
        }
        var matrix = Map.TerrainSection.MapMatrix;
        // modify column size
        while (matrix.Count > height)
        {
            matrix.RemoveAt(matrix.Count - 1);
        }
        while (matrix.Count < height)
        {
            List<MapTerrainEntry> entries = new();
            for (int i = 0; i < width; i++)
            {
                entries.Add(new MapTerrainEntry());
            }
            matrix.Add(entries);
        }
        // modify row size
        foreach (var row in matrix)
        {
            while (row.Count > width)
            {
                row.RemoveAt(row.Count - 1);
            }
            while (row.Count < width)
            {
                row.Add(new MapTerrainEntry());
            }
        }
        // ensure all gimmicks are in range
        foreach (var gimmick in Map.GimmickSection.Items)
        {
            if (!GetInRange(gimmick.Position))
            {
                gimmick.Position = new Position(0, 0);
            }
        }
        // ensure all maps are in range
        for (int i = 0; i < Map.PositionSection.Positions.Length; i++)
        {
            if (!GetInRange(Map.PositionSection.Positions[i]))
            {
                Map.PositionSection.Positions[i] = new Position(0, 0);
            }
        }
        // reset selected cell in case it's out of range
        SelectedCell = Matrix[0][0];
        Draw();
        RaisePropertyChanged(nameof(Width));
        RaisePropertyChanged(nameof(Height));
        
    }

    
}