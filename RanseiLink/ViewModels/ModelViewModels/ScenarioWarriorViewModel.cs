using GongSolutions.Wpf.DragDrop;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using RanseiLink.ValueConverters;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace RanseiLink.ViewModels;

public class ScenarioWarriorPokemonViewModel : ViewModelBase
{
    private readonly ScenarioWarrior _model;
    private readonly ScenarioWarriorViewModel _parent;
    public ScenarioWarriorPokemonViewModel(int id, ScenarioWarrior model, ScenarioWarriorViewModel parent)
    {
        Id = id;
        _parent = parent;
        _model = model;
        SetToDefaultCommand = new RelayCommand(() =>
        {
            ScenarioPokemonId = 1100;
        });
    }
    public int Id { get; }
    
    public int ScenarioPokemonId
    {
        get => _model.GetScenarioPokemon(Id);
        set 
        {
            if (ScenarioPokemonId != value)
            {
                value = CoerceValue(value);
                if (_parent.SelectedPokemon == this)
                {
                    _model.SetScenarioPokemon(Id, (ushort)value);
                    _parent.UpdateEmbedded(Id);
                }
                else
                {
                    _model.SetScenarioPokemon(Id, (ushort)value);
                }
                RaisePropertyChanged();
            } 
        }
    }

    private int CoerceValue(int value)
    {
        if (value < 0)
        {
            return 0;
        }
        if (value >= Core.Services.Constants.ScenarioPokemonCount)
        {
            if (value > ScenarioPokemonId)
            {
                value = 1100;
            }
            else
            {
                value = Core.Services.Constants.ScenarioPokemonCount - 1;
            }
        }
        return value;
    }

    public ICommand SetToDefaultCommand { get; }
}


public class ScenarioWarriorWorkspaceViewModel : ViewModelBase
{
    private readonly IScenarioWarriorService _scenarioWarriorService;
    private readonly ScenarioWarriorMiniViewModelFactory _itemFactory;
    private readonly ScenarioWarriorKingdomMiniViewModelFactory _kingdomItemFactory;
    public ScenarioWarriorWorkspaceViewModel(
        IScenarioWarriorService scenarioWarriorService, 
        ScenarioWarriorMiniViewModelFactory itemFactory,
        ScenarioWarriorKingdomMiniViewModelFactory kingdomItemFactory)
    {
        _scenarioWarriorService = scenarioWarriorService;
        _itemFactory = itemFactory;
        _kingdomItemFactory = kingdomItemFactory;
        ItemDragHandler = new DragHandlerPro();
    }

    public void SetModel(ScenarioId scenario, IChildScenarioWarriorService childScenarioWarriorService)
    {
        Items.Clear();
        foreach (var group in childScenarioWarriorService.Enumerate().GroupBy(x => x.Kingdom).OrderBy(x => x.Key))
        {
            Items.Add(_kingdomItemFactory(scenario, group.Key));
            foreach (var scenarioWarrior in group.OrderBy(x => x.Class))
            {
                var item = _itemFactory(scenarioWarrior, scenario, this);
                if (scenarioWarrior.ScenarioPokemonIsDefault(0))
                {
                    UnassignedItems.Add(item);
                }
                else
                {
                    Items.Add(item);
                }
                
            }
        }
    }

    public ObservableCollection<object> Items { get; } = new();
    public ObservableCollection<object> UnassignedItems { get; } = new();
    public DragHandlerPro ItemDragHandler { get; }
}

public class DragHandlerPro : DefaultDragHandler 
{
    public override bool CanStartDrag(IDragInfo dragInfo)
    {
        if (dragInfo.SourceItem is ScenarioWarriorMiniViewModel)
        {
            return base.CanStartDrag(dragInfo);
        }
        else
        {
            return false;
        }
    }
}

public delegate ScenarioWarriorMiniViewModel ScenarioWarriorMiniViewModelFactory(ScenarioWarrior model, ScenarioId scenario, ScenarioWarriorWorkspaceViewModel parent);

public delegate ScenarioWarriorKingdomMiniViewModel ScenarioWarriorKingdomMiniViewModelFactory(ScenarioId scenario, KingdomId kingdom);

public class ScenarioWarriorKingdomMiniViewModel : ViewModelBase
{
    private readonly ScenarioId _scenario;
    private readonly KingdomId _kingdom;
    private readonly ScenarioKingdom _scenarioKingdom;
    private readonly IOverrideDataProvider _spriteProvider;
    private readonly IBaseWarriorService _baseWarriorService;
    private readonly IChildScenarioWarriorService _scenarioWarriorService;
    private readonly IPokemonService _pokemonService;
    private readonly IChildScenarioPokemonService _scenarioPokemonService;
    public ScenarioWarriorKingdomMiniViewModel(ScenarioId scenario, KingdomId kingdom,
        IScenarioKingdomService scenarioKingdomService,
        IBaseWarriorService baseWarriorService,
        IScenarioWarriorService scenarioWarriorService,
        IScenarioPokemonService scenarioPokemonService,
        IOverrideDataProvider dataProvider, 
        IPokemonService pokemonService)
    {
        _kingdom = kingdom;
        _spriteProvider = dataProvider;
        _scenarioKingdom = scenarioKingdomService.Retrieve((int)scenario);
        _baseWarriorService = baseWarriorService;
        _scenarioWarriorService = scenarioWarriorService.Retrieve((int)scenario);
        _pokemonService = pokemonService;
        _scenarioPokemonService = scenarioPokemonService.Retrieve((int)scenario);
        UpdateKingdomImage();
        UpdateWarriorImage();
    }

    private void UpdateKingdomImage()
    {
        string spriteFile = _spriteProvider.GetSpriteFile(SpriteType.StlCastleIcon, (int)_kingdom).File;
        if (!PathToImageSourceConverter.TryConvert(spriteFile, out var img))
        {
            KingdomImage = null;
            return;
        }
        KingdomImage = img;
    }

    private ImageSource _KingdomImage;
    public ImageSource KingdomImage
    {
        get => _KingdomImage;
        set => RaiseAndSetIfChanged(ref _KingdomImage, value);
    }

    private ImageSource _warriorImage;
    public ImageSource WarriorImage
    {
        get => _warriorImage;
        set => RaiseAndSetIfChanged(ref _warriorImage, value);
    }

    private void UpdateWarriorImage()
    {
        int warrior = -1;
        foreach (var w in _scenarioWarriorService.Enumerate())
        {
            if (w.Class == WarriorClassId.ArmyLeader && w.Army == Army)
            {
                warrior = (int)w.Warrior;
                break;
            }
        }

        if (!_baseWarriorService.ValidateId(warrior))
        {
            WarriorImage = null;
            return;
        }
        int image = _baseWarriorService.Retrieve(warrior).Sprite;
        string spriteFile = _spriteProvider.GetSpriteFile(SpriteType.StlBushouS, image).File;
        if (!PathToImageSourceConverter.TryConvert(spriteFile, out var img))
        {
            WarriorImage = null;
            return;
        }
        WarriorImage = img;
    }

    public int Strength
    {
        get
        {
            int strength = 0;
            foreach (var warrior in _scenarioWarriorService.Enumerate())
            {
                if (warrior.Kingdom == _kingdom 
                    && warrior.Army == Army 
                    && (warrior.Class == WarriorClassId.ArmyLeader || warrior.Class == WarriorClassId.ArmyMember)
                    )
                {
                    if (warrior.ScenarioPokemonIsDefault(0))
                    {
                        continue;
                    }
                    strength += CalculateStrength(warrior.GetScenarioPokemon(0));
                }
            }
            return strength;
        }
    }

    private int CalculateStrength(int ScenarioPokemon)
    {
        var sp = _scenarioPokemonService.Retrieve(ScenarioPokemon);
        int pokemonId = (int)sp.Pokemon;
        if (!_pokemonService.ValidateId(pokemonId))
        {
            return 0;
        }
        var pokemon = _pokemonService.Retrieve(pokemonId);
        return StrengthCalculator.CalculateStrength(pokemon, (double)LinkCalculator.CalculateLink(sp.Exp));
    }

    public int Army
    {
        get => _kingdom == KingdomId.Default ? -1 : _scenarioKingdom.GetArmy(_kingdom);
        set 
        {
            if (_kingdom != KingdomId.Default)
            {
                RaiseAndSetIfChanged(Army, value, v => _scenarioKingdom.SetArmy(_kingdom, v));
            }
        }
    }
}

public class ScenarioWarriorMiniViewModel : ViewModelBase
{
    private readonly ScenarioId _scenario;
    private readonly ScenarioWarrior _model;
    private readonly IScenarioPokemonService _scenarioPokemonService;
    private readonly IOverrideDataProvider _spriteProvider;
    private readonly IBaseWarriorService _baseWarriorService;
    private readonly IPokemonService _pokemonService;

    public ScenarioWarriorMiniViewModel(ScenarioWarrior model, ScenarioId scenario, ScenarioWarriorWorkspaceViewModel parent,
        IScenarioPokemonService scenarioPokemonService, 
        IBaseWarriorService baseWarriorService, 
        IOverrideDataProvider dataProvider,
        IPokemonService pokemonService)
    {
        _model = model;
        _scenarioPokemonService = scenarioPokemonService;
        _spriteProvider = dataProvider;
        _scenario = scenario;
        _baseWarriorService = baseWarriorService;
        _pokemonService = pokemonService;
        UpdatePokemonImage();
        UpdateWarriorImage();
        SelectCommand = new RelayCommand(Select);
    }

    public ICommand SelectCommand { get; }

    private void Select()
    {

    }

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
            }
        }
    }
}

public class ScenarioWarriorViewModel : ViewModelBase
{
    private ScenarioWarrior _model;
    private ScenarioPokemon _currentScenarioPokemon;
    private ScenarioId _scenario;
    private readonly ScenarioPokemonViewModel _scenarioPokemonVm;
    private readonly IScenarioPokemonService _scenarioPokemonService;
    private ScenarioWarriorPokemonViewModel _selectedPokemon;

    public ScenarioWarriorViewModel(IJumpService jumpService, ScenarioPokemonViewModel newScenarioPokemonViewModel, IScenarioPokemonService scenarioPokemonService, IIdToNameService idToNameService)
    {
        _scenarioPokemonService = scenarioPokemonService;
        _scenarioPokemonVm = newScenarioPokemonViewModel;

        WarriorItems = idToNameService.GetComboBoxItemsExceptDefault<IBaseWarriorService>();
        KingdomItems = idToNameService.GetComboBoxItemsPlusDefault<IKingdomService>();
        ItemItems = idToNameService.GetComboBoxItemsPlusDefault<IItemService>();

        JumpToBaseWarriorCommand = new RelayCommand<int>(id => jumpService.JumpTo(BaseWarriorSelectorEditorModule.Id, id));
        JumpToMaxLinkCommand = new RelayCommand<int>(id => jumpService.JumpTo(MaxLinkSelectorEditorModule.Id, id));
        JumpToItemCommand = new RelayCommand<int>(id => jumpService.JumpTo(ItemSelectorEditorModule.Id, id));
        JumpToScenarioPokemon = new RelayCommand(() =>
        {
            if (ScenarioPokemonVm != null)
            {
                jumpService.JumpToNested(ScenarioPokemonSelectorEditorModule.Id, (int)_scenario, SelectedPokemon.ScenarioPokemonId);
            }
        });
    }

    public void SetModel(ScenarioId scenario, int id, ScenarioWarrior model)
    {
        _scenario = scenario;
        _model = model;
        Pokemon.Clear();
        for (int i = 0; i < 8; i++)
        {
            var swp = new ScenarioWarriorPokemonViewModel(i, _model, this);
            Pokemon.Add(swp);
        }

        SelectedPokemon = Pokemon[0];
        UpdateEmbedded(0);

        RaiseAllPropertiesChanged();
    }

    public ICommand JumpToBaseWarriorCommand { get; }
    public ICommand JumpToMaxLinkCommand { get; }
    public ICommand JumpToScenarioPokemon { get; }
    public ICommand JumpToItemCommand { get; }

    public List<SelectorComboBoxItem> WarriorItems { get; }
    public List<SelectorComboBoxItem> KingdomItems { get; }
    public List<SelectorComboBoxItem> ItemItems { get; }

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

    public int Item
    {
        get => (int)_model.Item;
        set => RaiseAndSetIfChanged(_model.Item, (ItemId)value, v => _model.Item = v);
    }

    public ObservableCollection<ScenarioWarriorPokemonViewModel> Pokemon { get; } = new ObservableCollection<ScenarioWarriorPokemonViewModel>();

    public ScenarioWarriorPokemonViewModel SelectedPokemon
    {
        get => _selectedPokemon;
        set
        {
            if (_selectedPokemon != value)
            {
                _selectedPokemon = value;
                RaisePropertyChanged();
                if (_selectedPokemon != null)
                {
                    UpdateEmbedded(value.Id);
                }
                
            }
        }
    }

    public void UpdateEmbedded(int warriorPokemonId)
    {
        if (_model.ScenarioPokemonIsDefault(warriorPokemonId))
        {
            ScenarioPokemonVm.SetModel(_scenario, 0, new ScenarioPokemon());
        }
        else
        {
            var spid = _model.GetScenarioPokemon(warriorPokemonId);
            _currentScenarioPokemon = _scenarioPokemonService.Retrieve((int)_scenario).Retrieve(spid);
            ScenarioPokemonVm.SetModel(_scenario, spid, _currentScenarioPokemon);
        }
    }

    public ScenarioPokemonViewModel ScenarioPokemonVm => _scenarioPokemonVm;
}
