using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace RanseiLink.ViewModels;

public delegate ScenarioAppearPokemonViewModel ScenarioAppearPokemonViewModelFactory(IScenarioAppearPokemon model);

public class AppearItem : ViewModelBase
{
    private readonly IScenarioAppearPokemon _model;
    public AppearItem(IScenarioAppearPokemon model, PokemonId id)
    {
        _model = model;
        Pokemon = id;
    }

    public bool CanAppear
    {
        get => _model.GetCanAppear(Pokemon);
        set => RaiseAndSetIfChanged(CanAppear, value, v => _model.SetCanAppear(Pokemon, v));
    }

    public PokemonId Pokemon { get; set; }
}

public class ScenarioAppearPokemonViewModel : ViewModelBase
{
    public ScenarioAppearPokemonViewModel(IScenarioAppearPokemon model)
    {
        AppearItems = EnumUtil.GetValuesExceptDefaults<PokemonId>().Select(i => new AppearItem(model, i)).ToList();
    }

    public List<AppearItem> AppearItems { get; }
}
