using RanseiLink.Core.Maps;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public enum MapRenderMode
{
    Terrain,
    Elevation
}

public class MapViewModel : ViewModelBase
{
    private static bool _terrainPaintingActive;
    private static TerrainId _terrainBrush;
    private static bool _elevationPaintingActive;
    private static float _elevationToPaint;
    private static bool _paintElevationEntireCell;
    private static MapRenderMode _mapRenderMode; // static so it's preserved between pages
    private static bool _hideGimmicks;
    private static bool _hidePokemonMarkers;
    private MapGimmickViewModel? _selectedGimmick;
    private MapPokemonPositionViewModel? _selectedPokemonPosition;
    private MapGridSubCellViewModel? _mouseOverItem;
    private MapGridCellViewModel? _selectedCell;
    private readonly IAsyncDialogService _dialogService;
    private readonly IGimmickService _gimmickService;
    private readonly IOverrideDataProvider _spriteProvider;
    private readonly IMapManager _mapManager;
    private readonly IMapViewerService _mapViewerService;
    private MapId _id;

    public event EventHandler? RequestSave;
    public event EventHandler? RequestReload;

    public PSLM Map { get; set; } = null!;

    public MapViewModel(
        IAsyncDialogService dialogService, 
        IGimmickService gimmickService, 
        IOverrideDataProvider overrideSpriteProvider, 
        IMapManager mapManager,
        IMapViewerService mapViewerService)
    {
        _mapManager = mapManager;
        _mapViewerService = mapViewerService;
        _dialogService = dialogService;
        _gimmickService = gimmickService;
        _spriteProvider = overrideSpriteProvider;
        
        RemoveSelectedGimmickCommand = new RelayCommand(RemoveSelectedGimmick, () => _selectedGimmick != null);
        ModifyMapDimensionsCommand = new RelayCommand(ModifyMapDimensions);
        ImportObjCommand = new RelayCommand(ImportObj);
        ExportObjCommand = new RelayCommand(ExportObj);
        ImportPacCommand = new RelayCommand(ImportPac);
        ExportPacCommand = new RelayCommand(ExportPac);
        ImportPslmCommand = new RelayCommand(ImportPslm);
        ExportPslmCommand = new RelayCommand(ExportPslm);
        RevertModelCommand = new RelayCommand(Revert3dModel, () => _mapManager.IsOverriden(_id));
        ModifyElevationToPaintCommand = new RelayCommand<string>(diff => { if (diff != null) ElevationToPaint += float.Parse(diff); });
        View3DModelCommand = new RelayCommand(View3DModel);

        this.PropertyChanged += MapViewModel_PropertyChanged;
    }

    private void MapViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedGimmick))
        {
            RemoveSelectedGimmickCommand.RaiseCanExecuteChanged();
        }
        else if (e.PropertyName == nameof(Is3dModelOverriden))
        {
            RevertModelCommand.RaiseCanExecuteChanged();
        }
    }

    public void SetModel(MapId id, PSLM model)
    {
        _id = id;
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
        RaisePropertyChanged(nameof(Is3dModelOverriden));
        RevertModelCommand.RaiseCanExecuteChanged();
        RemoveSelectedGimmickCommand.RaiseCanExecuteChanged();
        RaisePropertyChanged(nameof(Width));
        RaisePropertyChanged(nameof(Height));
    }
    public bool Is3dModelOverriden => _mapManager.IsOverriden(_id);
    public RelayCommand RevertModelCommand { get; }
    public ICommand ExportPslmCommand { get; }
    public ICommand ImportPslmCommand { get; }
    public ICommand ExportObjCommand { get; }
    public ICommand ImportObjCommand { get; }
    public ICommand ExportPacCommand { get; }
    public ICommand ImportPacCommand { get; }
    public ICommand ModifyElevationToPaintCommand { get; }

    public RelayCommand RemoveSelectedGimmickCommand { get; }
    public ICommand ModifyMapDimensionsCommand { get; }
    public ICommand View3DModelCommand { get; }
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

    public bool ElevationPaintingActive
    {
        get => _elevationPaintingActive;
        set => RaiseAndSetIfChanged(ref _elevationPaintingActive, value);
    }

    public float ElevationToPaint
    {
        get => _elevationToPaint;
        set => RaiseAndSetIfChanged(ref _elevationToPaint, value);
    }

    public bool PaintElevationEntireCell
    {
        get => _paintElevationEntireCell;
        set => RaiseAndSetIfChanged(ref _paintElevationEntireCell, value);
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

    public bool HideGimmicks
    {
        get => _hideGimmicks;
        set
        {
            if (RaiseAndSetIfChanged(ref _hideGimmicks, value))
            {
                Draw();
            }
        }
    }

    public bool HidePokemonMarkers
    {
        get => _hidePokemonMarkers;
        set
        {
            if (RaiseAndSetIfChanged(ref _hidePokemonMarkers, value))
            {
                Draw();
            }
        }
    }

    public ushort Width => (ushort)Map.TerrainSection.MapMatrix[0].Count;

    public ushort Height => (ushort)Map.TerrainSection.MapMatrix.Count;

    public ObservableCollection<List<MapGridCellViewModel>> Matrix { get; } = new();

    public MapGridSubCellViewModel? MouseOverItem
    {
        get => _mouseOverItem;
        set => RaiseAndSetIfChanged(ref _mouseOverItem, value);
    }

    public MapGimmickViewModel? SelectedGimmick
    {
        get => _selectedGimmick;
        set
        {
            if (RaiseAndSetIfChanged(ref _selectedGimmick, value))
            {
                if (value != null)
                {
                    SelectedCell = Matrix[value.Y][value.X];
                }
                RemoveSelectedGimmickCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public MapPokemonPositionViewModel? SelectedPokemonPosition
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

    public MapGridCellViewModel? SelectedCell
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
                var cellVm = new MapGridCellViewModel(col, x++, y, RenderMode, HideGimmicks, HidePokemonMarkers, _gimmickService, _spriteProvider);
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
            Matrix[gimmick.Y][gimmick.X].AddGimmick(gimmick);
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
        var sourceCell = Matrix[newGimmick.Y][newGimmick.X];
        sourceCell.AddGimmick(newGimmick);
        return newGimmick;
    }

    public void OnSubCellClicked(MapGridSubCellViewModel clickedSubCell)
    {
        SelectedCell = clickedSubCell.Parent;

        if (TerrainPaintingActive)
        {
            SelectedCell.Terrain = TerrainBrush;
        }
        if (ElevationPaintingActive)
        {
            if (PaintElevationEntireCell)
            {
                SelectedCell.SubCell0.Z = ElevationToPaint;
                SelectedCell.SubCell1.Z = ElevationToPaint;
                SelectedCell.SubCell2.Z = ElevationToPaint;
                SelectedCell.SubCell3.Z = ElevationToPaint;
                SelectedCell.SubCell4.Z = ElevationToPaint;
                SelectedCell.SubCell5.Z = ElevationToPaint;
                SelectedCell.SubCell6.Z = ElevationToPaint;
                SelectedCell.SubCell7.Z = ElevationToPaint;
                SelectedCell.SubCell8.Z = ElevationToPaint;
            }
            else
            {
                clickedSubCell.Z = ElevationToPaint;
            }
        }

        _selectedGimmick = SelectedCell.Gimmicks.FirstOrDefault();
        _selectedPokemonPosition = SelectedCell.Pokemon.FirstOrDefault();
        RaisePropertyChanged(nameof(SelectedGimmick));
        RaisePropertyChanged(nameof(SelectedPokemonPosition));
    }

    private void RemoveSelectedGimmick()
    {
        if (_selectedGimmick != null)
        {
            var sourceCell = Matrix[_selectedGimmick.Y][_selectedGimmick.X];
            sourceCell.RemoveGimmick(_selectedGimmick);
            Map.GimmickSection.Items.Remove(_selectedGimmick.GimmickItem);
            Gimmicks.Remove(_selectedGimmick);
            _selectedGimmick = null;
            RaisePropertyChanged(nameof(SelectedGimmick));
        }
    }

    private async void ModifyMapDimensions()
    {
        var vm = new ModifyMapDimensionsViewModel(Width, Height);
        if (!await _dialogService.ShowDialogWithResult(vm))
        {
            return;
        }
        ushort height = (ushort)vm.Height;
        ushort width = (ushort)vm.Width;
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

    public async void View3DModel()
    {
        await _mapViewerService.ShowDialog(_id);
    }

    public async void Revert3dModel()
    {
        await _mapManager.RevertModelToDefault(_id);
        RaisePropertyChanged(nameof(Is3dModelOverriden));
    }

    private async void ImportObj()
    {
        await _mapManager.ImportObj(_id);
        RaisePropertyChanged(nameof(Is3dModelOverriden));
    }

    private async void ExportObj()
    {
        await _mapManager.ExportObj(_id);
    }

    private async void ImportPac()
    {
        await _mapManager.ImportPac(_id);
        RaisePropertyChanged(nameof(Is3dModelOverriden));
    }

    private async void ExportPac()
    {
        await _mapManager.ExportPac(_id);
    }

    private async void ImportPslm()
    {
        if (!await _mapManager.ImportPslm(_id))
        {
            return;
        }
        RequestReload?.Invoke(this, EventArgs.Empty);
    }

    private async void ExportPslm()
    {
        RequestSave?.Invoke(this, EventArgs.Empty);
        if (!await _mapManager.ExportPslm(_id))
        {
            return;
        }
    }
}