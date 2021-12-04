using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using System.Collections.Generic;

namespace RanseiLink.ViewModels;

public delegate MaxLinkViewModel MaxLinkViewModelFactory(IMaxLink model);

public class WarriorMaxSyncListItem : ViewModelBase
{
    private readonly IMaxLink _model;
    public WarriorMaxSyncListItem(PokemonId pokemon, IMaxLink model)
    {
        _model = model;
        Pokemon = pokemon;
    }
    public uint MaxSyncValue
    {
        get => _model.GetMaxLink(Pokemon);
        set => RaiseAndSetIfChanged(MaxSyncValue, value, v => _model.SetMaxLink(Pokemon, v));
    }
    public PokemonId Pokemon { get; set; }
}

public class MaxLinkViewModel : ViewModelBase
{
    public MaxLinkViewModel(IMaxLink model)
    {
        var items = new List<WarriorMaxSyncListItem>();
        foreach (PokemonId pid in EnumUtil.GetValuesExceptDefaults<PokemonId>())
        {
            items.Add(new WarriorMaxSyncListItem(pid, model));
        }
        Items = items;
    }

    public IList<WarriorMaxSyncListItem> Items { get; }
}
