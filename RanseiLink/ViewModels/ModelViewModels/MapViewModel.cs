using RanseiLink.Core.Enums;
using RanseiLink.Core.Map;
using RanseiLink.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public delegate MapViewModel MapViewModelFactory(Map model);

public enum MapRenderMode
{
    Terrain,
    Elevation
}

public class MapViewModel : ViewModelBase
{
    private static bool _terrainPaintingActive;
    private static Terrain _terrainBrush;
    private static MapRenderMode _mapRenderMode; // static so it's preserved between pages
    private MapGimmickViewModel _selectedGimmick;
    private MapPokemonPositionViewModel _selectedPokemonPosition;
    private MapGridSubCellViewModel _mouseOverItem;
    private MapGridCellViewModel _selectedCell;
    private readonly IDialogService _dialogService;

    public Map Map { get; set; }

    public MapViewModel(IServiceContainer container, Map model)
    {
        _dialogService = container.Resolve<IDialogService>();
        Map = model;
        Gimmicks = new(Map.GimmickSection.Items.Select(i => new MapGimmickViewModel(this, i)));
        PokemonPositions = new();
        for (int i = 0; i < Map.PositionSection.Positions.Length; i++)
        {
            PokemonPositions.Add(new MapPokemonPositionViewModel(this, Map.PositionSection.Positions, i));
        }
        Draw();

        RemoveSelectedGimmickCommand = new RelayCommand(() =>
        {
            Map.GimmickSection.Items.Remove(_selectedGimmick.GimmickItem);
            Gimmicks.Remove(_selectedGimmick);
            _selectedGimmick = null;
        }, 
        () => _selectedGimmick != null);

        ModifyMapDimensionsCommand = new RelayCommand(() =>
        {
            var width = Width;
            var height = Height;
            if (_dialogService.ModifyMapDimensions(ref width, ref height))
            {
                Width = width;
                Height = height;
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
            }
        });
    }
    public ICommand RemoveSelectedGimmickCommand { get; }
    public ICommand ModifyMapDimensionsCommand { get; }
    public ObservableCollection<MapGimmickViewModel> Gimmicks { get; }
    public ObservableCollection<MapPokemonPositionViewModel> PokemonPositions { get; }

    public Terrain TerrainBrush
    {
        get => _terrainBrush;
        set => RaiseAndSetIfChanged(ref _terrainBrush, value);
    }

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

    public ushort Width
    {
        get => Map.Header.Width;
        set => RaiseAndSetIfChanged(Map.Header.Width, value, v => Map.Header.Width = v);
    }

    public ushort Height
    {
        get => Map.Header.Height;
        set => RaiseAndSetIfChanged(Map.Header.Height, value, v => Map.Header.Height = v);
    }

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
        uint y = 0;
        foreach (var row in Map.TerrainSection.MapMatrix)
        {
            uint x = 0;
            var rowItems = new List<MapGridCellViewModel>();
            foreach (var col in row)
            {
                var cellVm = new MapGridCellViewModel(col, x++, y, RenderMode);
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
}
