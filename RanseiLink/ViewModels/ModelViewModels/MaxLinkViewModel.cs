using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace RanseiLink.ViewModels;

public class WarriorMaxLinkListItem : ViewModelBase
{
    private readonly MaxLink _model;

    public WarriorMaxLinkListItem(int pokemonId, MaxLink model, string name, ImageSource sprite, int itemId)
    {
        _model = model;
        _pokemonId = pokemonId;
        Name = name;
        Sprite = sprite;
        Id = itemId;
    }
    public int MaxLinkValue
    {
        get => _model.GetMaxLink(_pokemonId);
        set => RaiseAndSetIfChanged(MaxLinkValue, value, v => _model.SetMaxLink(_pokemonId, v));
    }

    private int _pokemonId;

    /// <summary>
    /// This ID is sortable
    /// </summary>
    public int Id { get; }

    public string Name { get; }

    public ImageSource Sprite { get; }
}



public enum MaxLinkSortMode
{
    Id,
    Name,
    Link
}

public abstract class MaxLinkViewModelBase : ViewModelBase
{

    protected readonly IIdToNameService _idToNameService;
    protected readonly IOverrideDataProvider _overrideDataProvider;
    protected readonly ICachedSpriteProvider _spriteProvider;
    protected readonly IBaseWarriorService _baseWarriorService;

    protected readonly ICollectionView _itemsView;
    protected int _id;

    protected MaxLinkViewModelBase(IIdToNameService idToNameService,
        IOverrideDataProvider overrideDataProvider,
        ICachedSpriteProvider spriteProvider,
        IBaseWarriorService baseWarriorService)
    {
        _baseWarriorService = baseWarriorService;
        _spriteProvider = spriteProvider;
        _overrideDataProvider = overrideDataProvider;
        _idToNameService = idToNameService;

        _itemsView = CollectionViewSource.GetDefaultView(Items);
        SortItems = EnumUtil.GetValues<MaxLinkSortMode>().ToList();
    }

    protected void OnSetModel()
    {
        if (_selectedSortItem == MaxLinkSortMode.Link)
        {
            Sort();
        }
    }

    private void Sort()
    {
        var mode = _selectedSortItem;
        _itemsView.SortDescriptions.Clear();
        if (mode == MaxLinkSortMode.Id)
        {
            // default
        }
        else if (mode == MaxLinkSortMode.Name)
        {
            _itemsView.SortDescriptions.Add(new SortDescription(nameof(WarriorMaxLinkListItem.Name), ListSortDirection.Ascending));
        }
        else if (mode == MaxLinkSortMode.Link)
        {
            _itemsView.SortDescriptions.Add(new SortDescription(nameof(WarriorMaxLinkListItem.MaxLinkValue), ListSortDirection.Descending));
        }

        // this is always present, either for ID-only sorting
        // or as a "ThenBy" sort to give a consistent order to Name and MaxLinkValue sort value conflicts
        _itemsView.SortDescriptions.Add(new SortDescription(nameof(WarriorMaxLinkListItem.Id), ListSortDirection.Ascending));
    }

    public List<MaxLinkSortMode> SortItems { get; }

    private static MaxLinkSortMode _selectedSortItem = MaxLinkSortMode.Id;
    public MaxLinkSortMode SelectedSortItem
    {
        get => _selectedSortItem;
        set
        {
            if (RaiseAndSetIfChanged(ref _selectedSortItem, value))
            {
                Sort();
            }
        }
    }

    public ObservableCollection<WarriorMaxLinkListItem> Items { get; } = new ObservableCollection<WarriorMaxLinkListItem>();

    public abstract string SmallSpritePath { get; }
}

public class MaxLinkWarriorViewModel : MaxLinkViewModelBase
{
    public MaxLinkWarriorViewModel(IIdToNameService idToNameService,
        IOverrideDataProvider overrideDataProvider,
        ICachedSpriteProvider spriteProvider,
        IBaseWarriorService baseWarriorService
        ) : base(idToNameService, overrideDataProvider, spriteProvider, baseWarriorService)
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
            var sprite = _spriteProvider.GetSprite(SpriteType.StlPokemonS, pidInt);
            Items.Add(new WarriorMaxLinkListItem(pidInt, model, name, sprite, pidInt));
        }
        RaisePropertyChanged(nameof(SmallSpritePath));
        OnSetModel();
    }

    public override string SmallSpritePath 
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
        IBaseWarriorService baseWarriorService
        ) : base(idToNameService, overrideDataProvider, spriteProvider, baseWarriorService)
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
            var sprite = _spriteProvider.GetSprite(SpriteType.StlBushouS, warrior.Sprite);
            var model = maxLinkService.Retrieve(widInt);
            Items.Add(new WarriorMaxLinkListItem(id, model, name, sprite, widInt));
        }
        RaisePropertyChanged(nameof(SmallSpritePath));
        OnSetModel();
    }

    public override string SmallSpritePath => _overrideDataProvider.GetSpriteFile(SpriteType.StlPokemonM, _id).File;
}
