using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System.Collections.Generic;

namespace RanseiLink.ViewModels;

public interface IMaxLinkViewModel 
{ 
    void SetModel(MaxLink model); 
}

public class WarriorMaxSyncListItem : ViewModelBase
{
    private readonly MaxLink _model;
    private readonly PokemonId _pokemonId;
    public WarriorMaxSyncListItem(PokemonId pokemon, MaxLink model, string pokemonName)
    {
        _model = model;
        _pokemonId = pokemon;
        Pokemon = pokemonName;
    }
    public int MaxSyncValue
    {
        get => _model.GetMaxLink(_pokemonId);
        set => RaiseAndSetIfChanged(MaxSyncValue, value, v => _model.SetMaxLink(_pokemonId, v));
    }

    public string Pokemon { get; }
}

public class MaxLinkViewModel : ViewModelBase, IMaxLinkViewModel
{
    private readonly IIdToNameService _idToNameService;
    public MaxLinkViewModel(IIdToNameService idToNameService)
    {
        _idToNameService = idToNameService;
    }

    public void SetModel(MaxLink model)
    {
        var items = new List<WarriorMaxSyncListItem>();
        foreach (PokemonId pid in EnumUtil.GetValuesExceptDefaults<PokemonId>())
        {
            items.Add(new WarriorMaxSyncListItem(pid, model, _idToNameService.IdToName<IPokemonService>((int)pid)));
        }
        Items = items;
        RaiseAllPropertiesChanged();
    }

    public List<WarriorMaxSyncListItem> Items { get; private set; }
}
