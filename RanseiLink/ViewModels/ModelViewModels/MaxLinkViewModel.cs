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

namespace RanseiLink.ViewModels;

public class WarriorMaxLinkListItem : ViewModelBase
{
    private readonly MaxLink _model;
    private readonly PokemonId _pokemonId;
    public WarriorMaxLinkListItem(PokemonId pokemon, MaxLink model, string pokemonName)
    {
        _model = model;
        _pokemonId = pokemon;
        Pokemon = pokemonName;
    }
    public int MaxLinkValue
    {
        get => _model.GetMaxLink(_pokemonId);
        set => RaiseAndSetIfChanged(MaxLinkValue, value, v => _model.SetMaxLink(_pokemonId, v));
    }

    public PokemonId Id => _pokemonId;

    public string Pokemon { get; }
}

public enum MaxLinkSortMode
{
    Id,
    Name,
    Link
}

public class MaxLinkViewModel : ViewModelBase
{
    private int _id;
    private readonly IIdToNameService _idToNameService;
    private readonly IOverrideDataProvider _spriteProvider;
    private readonly ICollectionView _itemsView;
    public MaxLinkViewModel(IIdToNameService idToNameService, IOverrideDataProvider spriteProvider)
    {
        _spriteProvider = spriteProvider;
        _idToNameService = idToNameService;

        _itemsView = CollectionViewSource.GetDefaultView(Items);
        SortItems = EnumUtil.GetValues<MaxLinkSortMode>().ToList();
    }

    private void Sort(MaxLinkSortMode mode)
    {
        _itemsView.SortDescriptions.Clear();
        if (mode == MaxLinkSortMode.Id)
        {
            // default
        }
        else if (mode == MaxLinkSortMode.Name)
        {
            _itemsView.SortDescriptions.Add(new SortDescription(nameof(WarriorMaxLinkListItem.Pokemon), ListSortDirection.Ascending));
        }
        else if (mode == MaxLinkSortMode.Link)
        {
            _itemsView.SortDescriptions.Add(new SortDescription(nameof(WarriorMaxLinkListItem.MaxLinkValue), ListSortDirection.Descending));
        }
    }

    public void SetModel(int id, MaxLink model)
    {
        _id = id;
        Items.Clear();
        foreach (PokemonId pid in EnumUtil.GetValuesExceptDefaults<PokemonId>())
        {
            Items.Add(new WarriorMaxLinkListItem(pid, model, _idToNameService.IdToName<IPokemonService>((int)pid)));
        }
        RaisePropertyChanged(nameof(SmallSpritePath));
    }

    public List<MaxLinkSortMode> SortItems { get; }

    private MaxLinkSortMode _selectedSortItem = MaxLinkSortMode.Id;
    public MaxLinkSortMode SelectedSortItem
    {
        get => _selectedSortItem;
        set
        {
            if (RaiseAndSetIfChanged(ref _selectedSortItem, value))
            {
                Sort(_selectedSortItem);
            }
        }
    }

    public string SmallSpritePath => _spriteProvider.GetSpriteFile(SpriteType.StlBushouM, _id).File;

    public ObservableCollection<WarriorMaxLinkListItem> Items { get; } = new ObservableCollection<WarriorMaxLinkListItem>();
}
