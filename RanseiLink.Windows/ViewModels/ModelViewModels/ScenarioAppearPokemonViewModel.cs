#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Windows.Services;
using System.Collections.ObjectModel;

namespace RanseiLink.Windows.ViewModels;

public class AppearItem : ViewModelBase
{
    private readonly ScenarioAppearPokemon _model;
    private readonly PokemonId _id;
    public AppearItem(ScenarioAppearPokemon model, PokemonId id, string pokemonName)
    {
        _model = model;
        _id = id;
        Pokemon = pokemonName;
    }

    public bool CanAppear
    {
        get => _model.GetCanAppear(_id);
        set => RaiseAndSetIfChanged(CanAppear, value, v => _model.SetCanAppear(_id, v));
    }

    public string Pokemon { get; set; }
}

public class ScenarioAppearPokemonViewModel : ViewModelBase
{
    private readonly IIdToNameService _idToNameService;
    public ScenarioAppearPokemonViewModel(IIdToNameService idToNameService)
    {
        _idToNameService = idToNameService;
    }

    public void SetModel(ScenarioAppearPokemon model)
    {
        AppearItems.Clear();
        foreach (var item in EnumUtil.GetValuesExceptDefaults<PokemonId>().Select(i => new AppearItem(model, i, _idToNameService.IdToName<IPokemonService>((int)i))))
        {
            AppearItems.Add(item);  
        }
    }

    public ObservableCollection<AppearItem> AppearItems { get; } = new();
}
