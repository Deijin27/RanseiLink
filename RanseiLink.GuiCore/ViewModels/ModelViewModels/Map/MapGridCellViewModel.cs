using RanseiLink.Core.Maps;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;


public class MapGridCellViewModel : ViewModelBase
{
    public MapTerrainEntry TerrainEntry { get; }

    private bool _isSelected;

    private readonly IGimmickService _gimmickService;
    private readonly IOverrideDataProvider _spriteProvider;
    private readonly bool _showGimmicks;
    private readonly bool _showPokemonMarkers;
    public MapGridCellViewModel(MapTerrainEntry entry, int x, int y, bool showGimmicks, bool showPokemonMarkers, IGimmickService gimmickService, IOverrideDataProvider spriteProvider)
    {
        _gimmickService = gimmickService;
        _spriteProvider = spriteProvider;
        TerrainEntry = entry;
        _showGimmicks = showGimmicks;
        _showPokemonMarkers = showPokemonMarkers;

        X = x;
        Y = y;

        SubCell0 = new(this, 0);
        SubCell1 = new(this, 1);
        SubCell2 = new(this, 2);
        SubCell3 = new(this, 3);
        SubCell4 = new(this, 4);
        SubCell5 = new(this, 5);
        SubCell6 = new(this, 6);
        SubCell7 = new(this, 7);
        SubCell8 = new(this, 8);

        SubCells = [
            SubCell0, 
            SubCell1, 
            SubCell2, 
            SubCell3, 
            SubCell4, 
            SubCell5, 
            SubCell6, 
            SubCell7,
            SubCell8
            ];

        Pokemon.CollectionChanged += (s, e) =>
        {
            RaisePropertyChanged(nameof(PokemonMarkerVisibility));
            RaisePropertyChanged(nameof(PokemonMarkerText));
        };
    }

    public IEnumerable<MapGridSubCellViewModel> SubCells { get; }

    private Rgba32 _color;
    public Rgba32 Color
    {
        get => _color;
        set => SetProperty(ref _color, value);
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

    private void OnGimmickVmGimmickChanged(object? sender, EventArgs e)
    {
        UpdateVisibleGimmick();
    }

    private void UpdateVisibleGimmick()
    {
        RaisePropertyChanged(nameof(GimmickMarkerVisibility));
        if (_showGimmicks && Gimmicks.Any())
        {
            var gimmick = Gimmicks.Last();
            GimmickImagePath = _spriteProvider.GetSpriteFile(SpriteType.StlStageObje, _gimmickService.Retrieve((int)gimmick.Gimmick).Image1).File;
        }
        else
        {
            GimmickImagePath = null;
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public int X { get; }
    public int Y { get; }

    public TerrainId Terrain
    {
        get => TerrainEntry.Terrain;
        set => SetProperty(TerrainEntry.Terrain, value, v => TerrainEntry.Terrain = v);
    }

    public MapBounds Bounds
    {
        get => TerrainEntry.Unknown3;
        set => SetProperty(TerrainEntry.Unknown3, value, v => TerrainEntry.Unknown3 = v);
    }

    public int Unknown4
    {
        get => TerrainEntry.Unknown4;
        set => SetProperty(TerrainEntry.Unknown4, (byte)value, v => TerrainEntry.Unknown4 = v);
    }

    public OrientationAlt Orientation
    {
        get => TerrainEntry.Orientation;
        set => SetProperty(TerrainEntry.Orientation, value, v => TerrainEntry.Orientation = v);
    }

    public bool GimmickMarkerVisibility
    {
        get
        {
            if (!_showGimmicks)
            {
                return false;
            }
            else
            {
                return Gimmicks.Any();
            }
        }
    }

    public string? GimmickMarkerText
    {
        get
        {
            if (!_showGimmicks)
            {
                return null;
            }
            else
            {
                var gimmick = Gimmicks.FirstOrDefault();
                if (gimmick == null)
                {
                    return null;
                }
                return gimmick.Id.ToString();
            }
        }
    }

    public void UpdateGimmickMarkerText()
    {
        RaisePropertyChanged(nameof(GimmickMarkerText));
    }

    public bool PokemonMarkerVisibility
    {
        get
        {
            if (!_showPokemonMarkers)
            {
                return false;
            }
            else
            {
                return Pokemon.Any();
            }
        }
    }

    public string? PokemonMarkerText
    {
        get
        {
            if (!_showPokemonMarkers)
            {
                return null;
            }
            else
            {
                var pokemon = Pokemon.FirstOrDefault();
                if (pokemon == null)
                {
                    return null;
                }
                return pokemon.Id.ToString();
            }
        }
    }

    public ObservableCollection<MapGimmickViewModel> Gimmicks { get; } = [];

    public ObservableCollection<MapPokemonPositionViewModel> Pokemon { get; } = [];

    public string GimmicksString => string.Join(", ", Gimmicks.Select(i => i.Gimmick));

    private string? _gimmickImagePath;
    public string? GimmickImagePath
    {
        get => _gimmickImagePath;
        set => SetProperty(ref _gimmickImagePath, value);
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

