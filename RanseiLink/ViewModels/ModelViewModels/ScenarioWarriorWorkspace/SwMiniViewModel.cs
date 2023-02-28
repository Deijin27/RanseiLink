using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

namespace RanseiLink.ViewModels;

public class SwMiniViewModel : ViewModelBase
{
    public delegate SwMiniViewModel Factory();

    private ScenarioId _scenario;
    private ScenarioWarrior _model;
    private readonly IScenarioPokemonService _scenarioPokemonService;
    private readonly ICachedSpriteProvider _spriteProvider;
    private readonly IBaseWarriorService _baseWarriorService;
    private readonly IPokemonService _pokemonService;
    private ScenarioWarriorWorkspaceViewModel _parent;
    private readonly ScenarioPokemonViewModel.Factory _spVmFactory;

    public SwMiniViewModel(
        IScenarioPokemonService scenarioPokemonService,
        IBaseWarriorService baseWarriorService,
        ICachedSpriteProvider spriteProvider,
        IPokemonService pokemonService,
        IJumpService jumpService,
        ScenarioPokemonViewModel.Factory spVmFactory)
    {
        _spVmFactory = spVmFactory;
        _scenarioPokemonService = scenarioPokemonService;
        _spriteProvider = spriteProvider;
        _baseWarriorService = baseWarriorService;
        _pokemonService = pokemonService;

        JumpToBaseWarriorCommand = new RelayCommand<int>(id => jumpService.JumpTo(BaseWarriorSelectorEditorModule.Id, id));
        JumpToMaxLinkCommand = new RelayCommand<int>(id => jumpService.JumpTo(MaxLinkSelectorEditorModule.Id, id));
        JumpToItemCommand = new RelayCommand<int>(id => jumpService.JumpTo(ItemSelectorEditorModule.Id, id));
    }

    public SwMiniViewModel Init(ScenarioWarrior model, ScenarioId scenario, ScenarioWarriorWorkspaceViewModel parent, ScenarioPokemonViewModel spVm)
    {
        _model = model;
        _parent = parent;
        _scenario = scenario;
        _suppressUpdateNested = true;

        UpdateWarriorImage();

        ScenarioPokemonSlots.Clear();
        var spService = _scenarioPokemonService.Retrieve((int)scenario);
        for (int i = 0; i < 8; i++)
        {
            ScenarioPokemonSlots.Add(new SwPokemonSlotViewModel(this, scenario, model, i, spVm, spService, _spriteProvider));
        }
        SelectedItem = ScenarioPokemonSlots[0];

        _suppressUpdateNested = false;

        return this;
    }

    public ICommand JumpToBaseWarriorCommand { get; }
    public ICommand JumpToMaxLinkCommand { get; }
    public ICommand JumpToItemCommand { get; }
    public ICommand SelectCommand => _parent.ItemClickedCommand;
    public List<SelectorComboBoxItem> WarriorItems => _parent.WarriorItems;
    public List<SelectorComboBoxItem> ItemItems => _parent.ItemItems;
    public List<SelectorComboBoxItem> ScenarioPokemonItems => _parent.ScenarioPokemonItems;

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
        WarriorImage = _spriteProvider.GetSprite(SpriteType.StlBushouS, image);
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
                _parent.UpdateStrengths();
            }
        }
    }

    bool _suppressUpdateNested = false;
    private SwPokemonSlotViewModel _selectedItem;
    public SwPokemonSlotViewModel SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (RaiseAndSetIfChanged(ref _selectedItem, value))
            {
                if (!_suppressUpdateNested)
                {
                    value.UpdateNested();
                }
            }
        }
    }

    public ObservableCollection<SwPokemonSlotViewModel> ScenarioPokemonSlots { get; } = new ObservableCollection<SwPokemonSlotViewModel>();
}
