#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public class WarriorMaxLinkListItem : ViewModelBase
{
    private readonly MaxLink _model;
    private readonly Func<object> _sprite;
    private readonly Func<int, object?> _getLinkImg;

    public WarriorMaxLinkListItem(int pokemonId, MaxLink model, string name, Func<object> sprite, int itemId,
        IMaxLinkQuickSetter maxLinkQuickSetter, Func<int, object?> getLinkImg)
    {
        _model = model;
        _pokemonId = pokemonId;
        Name = name;
        _sprite = sprite;
        Id = itemId;
        MaxLinkQuickSetter = maxLinkQuickSetter;
        _getLinkImg = getLinkImg;
        UpdateLinkImg();
    }
    public int MaxLinkValue
    {
        get => _model.GetMaxLink(_pokemonId);
        set
        {
            // validate because this can come from the user's custom quick setter
            if (value >= 0 && value <= 100)
            {
                if (SetProperty(MaxLinkValue, value, v => _model.SetMaxLink(_pokemonId, v)))
                {
                    UpdateLinkImg();
                }
            }
        }
    }

    private void UpdateLinkImg()
    {
        LinkImg = _getLinkImg(MaxLinkValue);
    }

    public object? LinkImg
    {
        get => _linkImg;
        set => SetProperty(ref _linkImg, value);
    }

    private readonly int _pokemonId;
    private object? _linkImg;

    /// <summary>
    /// This ID is sortable
    /// </summary>
    public int Id { get; }
    public IMaxLinkQuickSetter MaxLinkQuickSetter { get; }
    public string Name { get; }

    public object Sprite => _sprite();
}

public interface IMaxLinkQuickSetter
{
    string ValuesSource { get; }
}


public enum MaxLinkSortMode
{
    Id,
    Name,
    Link
}

public abstract class MaxLinkViewModelBase : ViewModelBase, IMaxLinkQuickSetter
{

    protected readonly IIdToNameService _idToNameService;
    protected readonly IOverrideDataProvider _overrideDataProvider;
    protected readonly ICachedSpriteProvider _cachedSpriteProvider;
    protected readonly IBaseWarriorService _baseWarriorService;
    private readonly IPathToImageConverter _pathToImageConverter;
    private readonly CollectionSorter<WarriorMaxLinkListItem> _sorter;

    protected int _id;

    protected MaxLinkViewModelBase(IIdToNameService idToNameService,
        IOverrideDataProvider overrideDataProvider,
        ICachedSpriteProvider spriteProvider,
        IBaseWarriorService baseWarriorService,
        IPathToImageConverter pathToImageConverter)
    {
        _baseWarriorService = baseWarriorService;
        _pathToImageConverter = pathToImageConverter;
        _cachedSpriteProvider = spriteProvider;
        _overrideDataProvider = overrideDataProvider;
        _idToNameService = idToNameService;
        SortItems = EnumUtil.GetValues<MaxLinkSortMode>().ToList();

        LoadImages();

        Items = [];
        _sorter = new(Items);
    }

    private static object? __goldImage;
    private static object? __silverImage;
    private static object? __bronzeImage;
    private static bool __loaded;

    protected object? GetLinkImage(int value)
    {
        if (value < 50 || value > 100)
        {
            return null;
        }
        else if (value >= 90)
        {
            return __goldImage;
        }
        else if (value >= 70)
        {
            return __silverImage;
        }
        else
        {
            return __bronzeImage;
        }
    }

    private void LoadImages()
    {
        if (__loaded)
        {
            return;
        }

        __loaded = true;

        var images = _cachedSpriteProvider.GetClusterImages("graphics/ikusa/04_01_parts_ikusamap_lo.G2DR", [67, 74, 81]);
        __goldImage = images[0];
        __silverImage = images[1];
        __bronzeImage = images[2];
    }

    protected void OnSetModel()
    {
        Sort();
    }

    private void Sort()
    {
        var mode = _selectedSortItem;
        _sorter.Clear();

        if (mode == MaxLinkSortMode.Id)
        {
            // default
        }
        else if (mode == MaxLinkSortMode.Name)
        {
            _sorter.OrderBy(x => x.Name);
        }
        else if (mode == MaxLinkSortMode.Link)
        {
            _sorter.OrderByDescending(x => x.MaxLinkValue);
        }

        // this is always present, either for ID-only sorting
        // or as a "ThenBy" sort to give a consistent order to Name and MaxLinkValue sort value conflicts
        _sorter.OrderBy(x => x.Id);
        _sorter.ApplySort();
    }

    public List<MaxLinkSortMode> SortItems { get; }

    private static MaxLinkSortMode _selectedSortItem = MaxLinkSortMode.Id;
    public MaxLinkSortMode SelectedSortItem
    {
        get => _selectedSortItem;
        set
        {
            if (SetProperty(ref _selectedSortItem, value))
            {
                Sort();
            }
        }
    }

    public ObservableCollection<WarriorMaxLinkListItem> Items { get; }

    public abstract string? SmallSpritePath { get; }

    private static string _valuesSource = "0,50,70,90,100";
    public string ValuesSource
    {
        get => _valuesSource;
        set => SetProperty(ref _valuesSource, value);
    }
}

public class MaxLinkWarriorViewModel : MaxLinkViewModelBase
{
    public MaxLinkWarriorViewModel(IIdToNameService idToNameService,
        IOverrideDataProvider overrideDataProvider,
        ICachedSpriteProvider spriteProvider,
        IBaseWarriorService baseWarriorService,
        IPathToImageConverter pathToImageConverter
        ) : base(idToNameService, overrideDataProvider, spriteProvider, baseWarriorService, pathToImageConverter)
    {
        
    }

    public void SetModel(int id, MaxLink model)
    {
        _id = id;
        Items.Clear();
        foreach (PokemonId pid in EnumUtil.GetValuesExceptDefaults<PokemonId>())
        {
            var pidInt = (int)pid;
            var name = _idToNameService.IdToName<IPokemonService>(pidInt);
            var sprite = () => _cachedSpriteProvider.GetSprite(SpriteType.StlPokemonS, pidInt);
            Items.Add(new WarriorMaxLinkListItem(pidInt, model, name, sprite, pidInt, this, GetLinkImage));
        }
        RaisePropertyChanged(nameof(SmallSpritePath));
        OnSetModel();
    }

    public override string? SmallSpritePath 
    { 
        get
        {
            if (!_baseWarriorService.ValidateId(_id))
            {
                return null;
            }
            var spriteId = _baseWarriorService.Retrieve(_id).Sprite;
            return _overrideDataProvider.GetSpriteFile(SpriteType.StlBushouM, spriteId).File;
        } 
    }
    
}

public class MaxLinkPokemonViewModel : MaxLinkViewModelBase
{
    public MaxLinkPokemonViewModel(IIdToNameService idToNameService,
        IOverrideDataProvider overrideDataProvider,
        ICachedSpriteProvider spriteProvider,
        IBaseWarriorService baseWarriorService,
        IPathToImageConverter pathToImageConverter 
        ) : base(idToNameService, overrideDataProvider, spriteProvider, baseWarriorService, pathToImageConverter)
    {
    }

    public void SetModel(int id, IMaxLinkService maxLinkService)
    {
        _id = id;
        Items.Clear();
        foreach (WarriorId wid in EnumUtil.GetValuesExceptDefaults<WarriorId>())
        {
            var widInt = (int)wid;
            var warrior = _baseWarriorService.Retrieve(widInt);
            var name = _baseWarriorService.IdToName(widInt);
            var sprite = () => _cachedSpriteProvider.GetSprite(SpriteType.StlBushouS, warrior.Sprite);
            var model = maxLinkService.Retrieve(widInt);
            Items.Add(new WarriorMaxLinkListItem(id, model, name, sprite, widInt, this, GetLinkImage));
        }
        RaisePropertyChanged(nameof(SmallSpritePath));
        OnSetModel();
    }

    public override string? SmallSpritePath => _overrideDataProvider.GetSpriteFile(SpriteType.StlPokemonM, _id).File;
}
