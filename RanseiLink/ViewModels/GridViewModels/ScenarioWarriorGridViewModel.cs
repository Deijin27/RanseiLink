using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RanseiLink.ViewModels;

public class ScenarioWarriorGridViewModel : ViewModelBase
{
    private readonly IScenarioPokemonService _scenarioPokemonService;
    private readonly IPokemonService _pokemonService;
    private readonly IAbilityService _abilityService;

    public ScenarioWarriorGridViewModel(IIdToNameService idToNameService, IScenarioPokemonService scenarioPokemonService, IPokemonService pokemonService, IAbilityService abilityService)
    {
        _scenarioPokemonService = scenarioPokemonService;
        _pokemonService = pokemonService;
        _abilityService = abilityService;
        WarriorItems = idToNameService.GetComboBoxItemsExceptDefault<IBaseWarriorService>();
        KingdomItems = idToNameService.GetComboBoxItemsPlusDefault<IKingdomService>();
    }

    public void SetModel(int scenario, IChildScenarioWarriorService childScenarioWarriorService)
    {
        Items.Clear();
        foreach (int id in childScenarioWarriorService.ValidIds())
        {
            var sw = childScenarioWarriorService.Retrieve(id);
            string pokemon = "Default";
            string pokemonAbility = "Default";

            if (!sw.ScenarioPokemonIsDefault(0))
            {
                var spid = sw.GetScenarioPokemon(0);
                var childScenarioPokemonService = _scenarioPokemonService.Retrieve(scenario);
                if (childScenarioPokemonService.ValidateId(spid))
                {
                    var sp = childScenarioPokemonService.Retrieve(spid);
                    int pokemonId = (int)sp.Pokemon;
                    int abilityId = (int)sp.Ability;
                    if (_pokemonService.ValidateId(pokemonId))
                    {
                        pokemon = _pokemonService.IdToName(pokemonId);
                    }
                    if (_abilityService.ValidateId(abilityId))
                    {
                        pokemonAbility = _abilityService.IdToName(abilityId);
                    }
                }
            }

            Items.Add(new ScenarioWarriorGridItemViewModel(id, sw, pokemon, pokemonAbility));
        }
    }

    public ObservableCollection<ScenarioWarriorGridItemViewModel> Items { get; } = new ObservableCollection<ScenarioWarriorGridItemViewModel>();

    public List<SelectorComboBoxItem> WarriorItems { get; }
    public List<SelectorComboBoxItem> KingdomItems { get; }
}

public class ScenarioWarriorGridItemViewModel : ViewModelBase
{
    private readonly ScenarioWarrior _model;

    public ScenarioWarriorGridItemViewModel(int id, ScenarioWarrior model, string pokemon, string pokemonAbility)
    {
        Id = id;
        _model = model;
        Pokemon = pokemon;
        PokemonAbility = pokemonAbility;
    }

    public int Id { get; }

    public int Warrior
    {
        get => (int)_model.Warrior;
        set => RaiseAndSetIfChanged(_model.Warrior, (WarriorId)value, v => _model.Warrior = v);
    }

    public WarriorClassId Class
    {
        get => _model.Class;
        set => RaiseAndSetIfChanged(_model.Class, value, v => _model.Class = v);
    }

    public int Kingdom
    {
        get => (int)_model.Kingdom;
        set => RaiseAndSetIfChanged(_model.Kingdom, (KingdomId)value, v => _model.Kingdom = v);
    }

    public int Army
    {
        get => _model.Army;
        set => RaiseAndSetIfChanged(_model.Army, value, v => _model.Army = v);
    }

    public string Pokemon { get; }

    public string PokemonAbility { get; }
}