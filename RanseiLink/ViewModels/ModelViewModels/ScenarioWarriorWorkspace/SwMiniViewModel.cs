using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.ValueConverters;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

namespace RanseiLink.ViewModels;

public delegate SwMiniViewModel SwMiniViewModelFactory();

public class SwMiniViewModel : ViewModelBase
{
    private ScenarioId _scenario;
    private ScenarioWarrior _model;
    private readonly IScenarioPokemonService _scenarioPokemonService;
    private readonly IOverrideDataProvider _spriteProvider;
    private readonly IBaseWarriorService _baseWarriorService;
    private readonly IPokemonService _pokemonService;
    private ScenarioWarriorWorkspaceViewModel _parent;

    public SwMiniViewModel(
        IScenarioPokemonService scenarioPokemonService, 
        IBaseWarriorService baseWarriorService, 
        IOverrideDataProvider dataProvider,
        IPokemonService pokemonService)
    {
        _scenarioPokemonService = scenarioPokemonService;
        _spriteProvider = dataProvider;
        _baseWarriorService = baseWarriorService;
        _pokemonService = pokemonService;        
    }

    public void Init(ScenarioWarrior Model, ScenarioId Scenario, ScenarioWarriorWorkspaceViewModel Parent, ScenarioPokemonViewModel SpVm)
    {
        _model = Model;
        _parent = Parent;
        _scenario = Scenario;
        UpdatePokemonImage();
        UpdateWarriorImage();

        for (int i = 0; i < 8; i++)
        {
            ScenarioPokemonSlots.Add(new SwPokemonSlotViewModel(Scenario, Model, i, SpVm, _scenarioPokemonService.Retrieve((int)Scenario), _spriteProvider));
        }
    }

    public ICommand SelectCommand => _parent.ItemClickedCommand;
    public List<SelectorComboBoxItem> WarriorItems => _parent.WarriorItems;
    public List<SelectorComboBoxItem> ItemItems => _parent.ItemItems;

    public string WarriorName => _baseWarriorService.IdToName(Warrior);

    public int Warrior
    {
        get => (int)_model.Warrior;
        set
        {
            if (RaiseAndSetIfChanged(_model.Warrior, (WarriorId)value, v => _model.Warrior = v))
            {
                RaisePropertyChanged(WarriorName);
                UpdateWarriorImage();
            }
        }
    }

    public int Strength
    {
        get
        {
            if (_model.ScenarioPokemonIsDefault(0))
            {
                return 0;
            }
            var sp = _scenarioPokemonService.Retrieve((int)_scenario).Retrieve(ScenarioPokemon);
            int pokemonId = (int)sp.Pokemon;
            if (!_pokemonService.ValidateId(pokemonId))
            {
                return 0;
            }
            var pokemon = _pokemonService.Retrieve(pokemonId);
            return StrengthCalculator.CalculateStrength(pokemon, (double)LinkCalculator.CalculateLink(sp.Exp));
        }
    }

    public WarriorClassId Class
    {
        get => _model.Class;
        set
        {
            var oldClass = _model.Class;
            if (RaiseAndSetIfChanged(_model.Class, value, v => _model.Class = v))
            {
                if (Class == WarriorClassId.ArmyLeader || oldClass == WarriorClassId.ArmyLeader)
                {
                    _parent.UpdateLeaders();
                }
            }
        }
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

    public int Item
    {
        get => (int)_model.Item;
        set => RaiseAndSetIfChanged(_model.Item, (ItemId)value, v => _model.Item = v);
    }

    private ImageSource _warriorImage;
    public ImageSource WarriorImage
    {
        get => _warriorImage;
        set => RaiseAndSetIfChanged(ref _warriorImage, value);
    }

    private void UpdateWarriorImage()
    {
        if (!_baseWarriorService.ValidateId(Warrior))
        {
            WarriorImage = null;
            return;
        }
        int image = _baseWarriorService.Retrieve(Warrior).Sprite;
        string spriteFile = _spriteProvider.GetSpriteFile(SpriteType.StlBushouS, image).File;
        if (!PathToImageSourceConverter.TryConvert(spriteFile, out var img))
        {
            WarriorImage = null;
            return;
        }
        WarriorImage = img;
    }

    private ImageSource _pokemonImage;
    public ImageSource PokemonImage
    {
        get => _pokemonImage;
        set => RaiseAndSetIfChanged(ref _pokemonImage, value);
    }

    private void UpdatePokemonImage()
    {
        if (_model.ScenarioPokemonIsDefault(0))
        {
            PokemonImage = null;
            return;
        }
        int image = (int)_scenarioPokemonService.Retrieve((int)_scenario).Retrieve(ScenarioPokemon).Pokemon;
        string spriteFile = _spriteProvider.GetSpriteFile(SpriteType.StlPokemonS, image).File;
        if (!PathToImageSourceConverter.TryConvert(spriteFile, out var img))
        {
            PokemonImage = null;
            return;
        }
        
        PokemonImage = img;
    }

    public int ScenarioPokemon
    {
        get => _model.GetScenarioPokemon(0);
        set
        {
            if (ScenarioPokemon != value)
            {
                _model.SetScenarioPokemon(0, value);
                RaisePropertyChanged();
                UpdatePokemonImage();
                _parent.UpdateStrengths();
            }
        }
    }

    public ObservableCollection<SwPokemonSlotViewModel> ScenarioPokemonSlots { get; } = new ObservableCollection<SwPokemonSlotViewModel>();
}
