#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;

namespace RanseiLink.Windows.ViewModels;

public class ScenarioPokemonViewModel : ViewModelBase
{
    public delegate ScenarioPokemonViewModel Factory();

    private readonly IPokemonService _pokemonService;
    private ScenarioPokemon _model;
    private int _id;
    private ScenarioId _scenario;
    private readonly List<SelectorComboBoxItem> _allAbilityItems;
    private string _ivQuickSetSource = "0,5,10,15,20,25,31";
    private string _linkQuickSetSource = "0,10,20,30";

    public string IvQuickSetSource
    {
        get => _ivQuickSetSource;
        set => RaiseAndSetIfChanged(ref _ivQuickSetSource, value);
    }

    public string LinkQuickSetSource
    {
        get => _linkQuickSetSource;
        set => RaiseAndSetIfChanged(ref _linkQuickSetSource, value);
    }

    public ScenarioPokemonViewModel(
        IJumpService jumpService,
        IIdToNameService idToNameService,
        IPokemonService pokemonService)
    {
        _pokemonService = pokemonService;
        _model = new ScenarioPokemon();

        JumpToPokemonCommand = new RelayCommand<int>(id => jumpService.JumpTo(PokemonWorkspaceModule.Id, id));
        JumpToAbilityCommand = new RelayCommand<int>(id => jumpService.JumpTo(AbilitySelectorEditorModule.Id, id));

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

    private bool ValidateIv(int value)
    {
        return value >= 0 && value <= 31;
    }

    public int HpIv
    {
        get => _model.HpIv;
        set
        {
            if (ValidateIv(value))
            {
                RaiseAndSetIfChanged(_model.HpIv, value, v => _model.HpIv = v);
            }
        }
    }

    public int AtkIv
    {
        get => _model.AtkIv;
        set
        {
            if (ValidateIv(value))
            {
                RaiseAndSetIfChanged(_model.AtkIv, value, v => _model.AtkIv = v);
            }
        }
    }

    public int DefIv
    {
        get => _model.DefIv;
        set
        {
            if (ValidateIv(value))
            {
                RaiseAndSetIfChanged(_model.DefIv, value, v => _model.DefIv = v);
            }
        }
    }

    public int SpeIv
    {
        get => _model.SpeIv;
        set
        {
            if (ValidateIv(value))
            {
                RaiseAndSetIfChanged(_model.SpeIv, value, v => _model.SpeIv = v);
            }
        }
    }

    // the default values are all integers, so I decided to drop support for setting exp directly
    // to simplify the ui
    public int Link
    {
        get => (int)LinkCalculator.CalculateLink(_model.Exp);
        set
        {
            if (value >= 0 && value <= 100)
            {
                var newValue = LinkCalculator.CalculateExp(value);
                if (newValue != _model.Exp)
                {
                    _model.Exp = newValue;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
