using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class ScenarioPokemonViewModel : ViewModelBase
{
    private IPokemonService _pokemonService;
    private ScenarioPokemon _model;
    private int _id;
    private ScenarioId _scenario;
    private readonly List<SelectorComboBoxItem> _allAbilityItems;

    private static int _linkNum;
    public int LinkNum
    {
        get => _linkNum;
        set => RaiseAndSetIfChanged(ref _linkNum, value);
    }
    public ICommand SetExactLinkCommand { get; }

    public ScenarioPokemonViewModel(
        IJumpService jumpService,
        IScenarioWarriorService scenarioWarriorService,
        IIdToNameService idToNameService,
        IPokemonService pokemonService)
    {
        _pokemonService = pokemonService;
        _model = new ScenarioPokemon();

        SetExactLinkCommand = new RelayCommand(() => Exp = LinkCalculator.CalculateExp(LinkNum));
        JumpToPokemonCommand = new RelayCommand<int>(id => jumpService.JumpTo(PokemonSelectorEditorModule.Id, id));
        JumpToAbilityCommand = new RelayCommand<int>(id => jumpService.JumpTo(AbilitySelectorEditorModule.Id, id));
        JumpToFirstWarriorCommand = new RelayCommand(() =>
        {
            int i = 0;
            foreach (var sw in scenarioWarriorService.Retrieve((int)_scenario).Enumerate())
            {
                if (!sw.ScenarioPokemonIsDefault(0) && sw.GetScenarioPokemon(0) == _id)
                {
                    jumpService.JumpToNested(ScenarioWarriorSelectorEditorModule.Id, (int)_scenario, i);
                    return;
                }
                i++;
            }
        });

        PokemonItems = idToNameService.GetComboBoxItemsExceptDefault<IPokemonService>();
        PokemonItems.Add(new SelectorComboBoxItem(511, "Default"));
        _allAbilityItems = idToNameService.GetComboBoxItemsPlusDefault<IAbilityService>();
    }

    private void ReloadAbilities()
    {
        AbilityItems.Clear();
        if (_model.Pokemon == PokemonId.Default)
        {
            return;
        }
        var pokemon = _pokemonService.Retrieve((int)_model.Pokemon);
        foreach (var abilityItem in _allAbilityItems)
        {
            var abilityId = (AbilityId)abilityItem.Id;

            if (pokemon.Ability1 == abilityId || pokemon.Ability2 == abilityId || pokemon.Ability3 == abilityId)
            {
                AbilityItems.Insert(0, abilityItem);
            }
            else
            {
                AbilityItems.Add(abilityItem);
            }
        }
    }

    public void SetModel(ScenarioId scenario, int id, ScenarioPokemon model)
    {
        _model = model;
        _scenario = scenario;
        _id = id;
        ReloadAbilities();
        RaiseAllPropertiesChanged();
    }

    public ICommand JumpToPokemonCommand { get; }
    public ICommand JumpToAbilityCommand { get; }

    /// <summary>
    /// Jump to first scenario warrior with this scenario pokemon
    /// </summary>
    public ICommand JumpToFirstWarriorCommand { get; }

    public List<SelectorComboBoxItem> PokemonItems { get; }
    public ObservableCollection<SelectorComboBoxItem> AbilityItems { get; } = new ObservableCollection<SelectorComboBoxItem>();

    public int Pokemon
    {
        get => (int)_model.Pokemon;
        set
        {
            if (RaiseAndSetIfChanged(_model.Pokemon, (PokemonId)value, v => _model.Pokemon = v))
            {
                ReloadAbilities();
            }
        }
    }

    public int Ability
    {
        get => (int)_model.Ability;
        set => RaiseAndSetIfChanged(_model.Ability, (AbilityId)value, v => _model.Ability = v);
    }

    public int HpIv
    {
        get => _model.HpIv;
        set => RaiseAndSetIfChanged(_model.HpIv, value, v => _model.HpIv = v);
    }

    public int AtkIv
    {
        get => _model.AtkIv;
        set => RaiseAndSetIfChanged(_model.AtkIv, value, v => _model.AtkIv = v);
    }

    public int DefIv
    {
        get => _model.DefIv;
        set => RaiseAndSetIfChanged(_model.DefIv, value, v => _model.DefIv = v);
    }

    public int SpeIv
    {
        get => _model.SpeIv;
        set => RaiseAndSetIfChanged(_model.SpeIv, value, v => _model.SpeIv = v);
    }

    public int Exp
    {
        get => _model.Exp;
        set
        {
            if (RaiseAndSetIfChanged(_model.Exp, value, v => _model.Exp = (ushort)v))
            {
                RaisePropertyChanged(nameof(ApproximateLink));
            }
        }
    }

    public string ApproximateLink
    {
        get => LinkCalculator.CalculateLink(_model.Exp).ToString("0.00");
        set => Exp = LinkCalculator.CalculateExp(double.Parse(value));
    }
}
