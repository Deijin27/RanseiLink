using GongSolutions.Wpf.DragDrop;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using RanseiLink.ValueConverters;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    private bool _loading;
    private readonly SwMiniViewModelFactory _itemFactory;
    private readonly SwKingdomMiniViewModelFactory _kingdomItemFactory;
    private readonly SwSimpleKingdomMiniViewModelFactory _simpleKingdomItemFactory;
    private static bool _showArmy = true;
    private static bool _showFree = false;
    private static bool _showUnassigned = false;
    private SwMiniViewModel _selectedItem;

    public ScenarioWarriorWorkspaceViewModel(
        SwMiniViewModelFactory itemFactory,
        SwKingdomMiniViewModelFactory kingdomItemFactory,
        SwSimpleKingdomMiniViewModelFactory simpleKingdomItemFactory,
        IIdToNameService idToNameService)
    {
        _itemFactory = itemFactory;
        _kingdomItemFactory = kingdomItemFactory;
        _simpleKingdomItemFactory = simpleKingdomItemFactory;
        ItemDragHandler = new DragHandlerPro();
        ItemDropHandler = new DropHandlerPro();
        ItemClickedCommand = new RelayCommand<object>(ItemClicked);

        WarriorItems = idToNameService.GetComboBoxItemsExceptDefault<IBaseWarriorService>();
        KingdomItems = idToNameService.GetComboBoxItemsPlusDefault<IKingdomService>();
        ItemItems = idToNameService.GetComboBoxItemsPlusDefault<IItemService>();

        Items.CollectionChanged += Items_CollectionChanged;
    }

    private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (_loading)
        {
            return;
        }

        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            var newItem = (SwMiniViewModel)e.NewItems[0];
            var newIndex = e.NewStartingIndex;
        }
    }

    private SwKingdomMiniViewModel GetKingdom(int index)
    {
        // walk up the list from this index.
        return null;
    }

    public void SetModel(ScenarioId scenario, IChildScenarioWarriorService childScenarioWarriorService)
    {
        _loading = true;

        Items.Clear();
        UnassignedItems.Clear();
        WildItems.Clear();
        foreach (var group in childScenarioWarriorService.Enumerate().GroupBy(x => x.Kingdom).OrderBy(x => x.Key))
        {
            Items.Add(_kingdomItemFactory(scenario, group.Key));
            WildItems.Add(_simpleKingdomItemFactory(group.Key));
            foreach (var scenarioWarrior in group.OrderBy(x => x.Class))
            {
                var item = _itemFactory(scenarioWarrior, scenario, this);
                switch (scenarioWarrior.Class)
                {
                    case WarriorClassId.ArmyLeader:
                    case WarriorClassId.ArmyMember:
                        Items.Add(item);
                        break;
                    case WarriorClassId.FreeWarrior:
                    case WarriorClassId.Unknown_3:
                    case WarriorClassId.Unknown_4:
                        WildItems.Add(item);
                        break;
                    case WarriorClassId.Default:
                        UnassignedItems.Add(item);
                        break;
                    default:
                        throw new System.Exception("Unexpected warrior class id");
                }

            }
        }
        SelectedItem = Items.OfType<SwMiniViewModel>().FirstOrDefault();

        _loading = false;
    }

    public ObservableCollection<object> Items { get; } = new();
    public ObservableCollection<object> WildItems { get; } = new();
    public ObservableCollection<object> UnassignedItems { get; } = new();
    public DragHandlerPro ItemDragHandler { get; }
    public DropHandlerPro ItemDropHandler { get; }
    public ICommand ItemClickedCommand { get; }
    public List<SelectorComboBoxItem> WarriorItems { get; }
    public List<SelectorComboBoxItem> KingdomItems { get; }
    public List<SelectorComboBoxItem> ItemItems { get; }

    public bool ShowArmy
    {
        get => _showArmy;
        set => RaiseAndSetIfChanged(ref _showArmy, value);
    }
    public bool ShowFree
    {
        get => _showFree;
        set => RaiseAndSetIfChanged(ref _showFree, value);
    }
    public bool ShowUnassigned
    {
        get => _showUnassigned;
        set => RaiseAndSetIfChanged(ref _showUnassigned, value);
    }
    public SwMiniViewModel SelectedItem
    {
        get => _selectedItem;
        set => RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    private void ItemClicked(object sender)
    {
        if (sender is not SwMiniViewModel clickedVm)
        {
            return;
        }
        SelectedItem = clickedVm;
    }
}

public class DragHandlerPro : DefaultDragHandler 
{
    public override bool CanStartDrag(IDragInfo dragInfo)
    {
        if (dragInfo.SourceItem is SwMiniViewModel)
        {
            return base.CanStartDrag(dragInfo);
        }
        else
        {
            return false;
        }
    }
}

public class DropHandlerPro : DefaultDropHandler
{
    public override void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.InsertIndex == 0)
        {
            return;
        }
        base.DragOver(dropInfo);
    }
}

public delegate SwMiniViewModel SwMiniViewModelFactory(ScenarioWarrior model, ScenarioId scenario, ScenarioWarriorWorkspaceViewModel parent);

public delegate SwKingdomMiniViewModel SwKingdomMiniViewModelFactory(ScenarioId scenario, KingdomId kingdom);

public delegate SwSimpleKingdomMiniViewModel SwSimpleKingdomMiniViewModelFactory(KingdomId kingdom);

public class SwSimpleKingdomMiniViewModel : ViewModelBase
{
    private readonly KingdomId _kingdom;
    protected readonly IOverrideDataProvider _spriteProvider;
    public SwSimpleKingdomMiniViewModel(KingdomId kingdom, IOverrideDataProvider spriteProvider)
    {
        _kingdom = kingdom;
        _spriteProvider = spriteProvider;
        UpdateKingdomImage();
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
}

public class SwKingdomMiniViewModel : SwSimpleKingdomMiniViewModel
{
    private readonly KingdomId _kingdom;
    private readonly ScenarioKingdom _scenarioKingdom;
    
    private readonly IBaseWarriorService _baseWarriorService;
    private readonly IChildScenarioWarriorService _scenarioWarriorService;
    private readonly IPokemonService _pokemonService;
    private readonly IChildScenarioPokemonService _scenarioPokemonService;
    public SwKingdomMiniViewModel(ScenarioId scenario, KingdomId kingdom,
        IScenarioKingdomService scenarioKingdomService,
        IBaseWarriorService baseWarriorService,
        IScenarioWarriorService scenarioWarriorService,
        IScenarioPokemonService scenarioPokemonService,
        IOverrideDataProvider dataProvider, 
        IPokemonService pokemonService) : base(kingdom, dataProvider)
    {
        _kingdom = kingdom;
        _scenarioKingdom = scenarioKingdomService.Retrieve((int)scenario);
        _baseWarriorService = baseWarriorService;
        _scenarioWarriorService = scenarioWarriorService.Retrieve((int)scenario);
        _pokemonService = pokemonService;
        _scenarioPokemonService = scenarioPokemonService.Retrieve((int)scenario);
        UpdateWarriorImage();
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

public class SwMiniViewModel : ViewModelBase
{
    private readonly ScenarioId _scenario;
    private readonly ScenarioWarrior _model;
    private readonly IScenarioPokemonService _scenarioPokemonService;
    private readonly IOverrideDataProvider _spriteProvider;
    private readonly IBaseWarriorService _baseWarriorService;
    private readonly IPokemonService _pokemonService;
    private ScenarioWarriorWorkspaceViewModel _parent;

    public SwMiniViewModel(ScenarioWarrior model, ScenarioId scenario, ScenarioWarriorWorkspaceViewModel parent,
        IScenarioPokemonService scenarioPokemonService, 
        IBaseWarriorService baseWarriorService, 
        IOverrideDataProvider dataProvider,
        IPokemonService pokemonService)
    {
        _model = model;
        _parent = parent;
        _scenarioPokemonService = scenarioPokemonService;
        _spriteProvider = dataProvider;
        _scenario = scenario;
        _baseWarriorService = baseWarriorService;
        _pokemonService = pokemonService;
        UpdatePokemonImage();
        UpdateWarriorImage();
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
