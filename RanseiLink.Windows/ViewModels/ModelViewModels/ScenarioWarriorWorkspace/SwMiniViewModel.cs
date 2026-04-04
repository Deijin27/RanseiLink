#nullable enable
using FactoryGenie;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;

namespace RanseiLink.Windows.ViewModels;

[GenerateFactory]
public class SwMiniViewModel : ViewModelBase
{
    private readonly ScenarioId _scenario;
    private readonly ScenarioWarrior _model;
    private readonly ScenarioWarriorWorkspaceViewModel _parent;
    bool _suppressUpdateNested = false;
    private SwPokemonSlotViewModel? _selectedItem;
    private readonly IScenarioPokemonService _scenarioPokemonService;
    private readonly IBaseWarriorService _baseWarriorService;
    private readonly ICachedSpriteProvider _spriteProvider;
    private readonly IKingdomService _kingdomService;
    private readonly IStrengthService _strengthService;
    private readonly ScenarioWarrior _model1;
    private readonly ScenarioId _scenario1;
    private readonly ScenarioPokemonViewModel _spVm;

    public SwMiniViewModel(
        IScenarioPokemonService scenarioPokemonService,
        IBaseWarriorService baseWarriorService,
        ICachedSpriteProvider spriteProvider,
        IKingdomService kingdomService,
        IStrengthService strengthService,
        [FactoryParam] int id,
        [FactoryParam] ScenarioWarrior model,
        [FactoryParam] ScenarioId scenario,
        [FactoryParam] ScenarioWarriorWorkspaceViewModel parent,
        [FactoryParam] ScenarioPokemonViewModel spVm
    )
    {
        _scenarioPokemonService = scenarioPokemonService;
        _baseWarriorService = baseWarriorService;
        _spriteProvider = spriteProvider;
        _kingdomService = kingdomService;
        _strengthService = strengthService;
        _model1 = model;
        _scenario1 = scenario;
        _spVm = spVm;
        Id = id;
        _model = model;
        _parent = parent;
        _scenario = scenario;

        _suppressUpdateNested = true;

        UpdateWarriorImage();

        ScenarioPokemonSlots.Clear();
        var spService = _scenarioPokemonService.Retrieve((int)_scenario1);
        for (int i = 0; i < 8; i++)
        {
            var slotVm = new SwPokemonSlotViewModel(_parent.ScenarioPokemonItems, _scenario1, _model1, i, _spVm, spService, _spriteProvider);
            slotVm.PropertyChanged += SlotVm_PropertyChanged;
            ScenarioPokemonSlots.Add(slotVm);
        }
        SelectedItem = ScenarioPokemonSlots[0];

        _suppressUpdateNested = false;
    }

    public int Id { get; private set; }

    private void SlotVm_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(SwPokemonSlotViewModel.ScenarioPokemonId))
        {
            return;
        }
        if (sender is not SwPokemonSlotViewModel slotVm)
        {
            return;
        }
        UpdateStrength();
        _parent.UpdateScenarioPokemonComboItemName(slotVm.ScenarioPokemonId);
    }

    public ICommand JumpToBaseWarriorCommand => _parent.JumpToBaseWarriorCommand;
    public ICommand JumpToMaxLinkCommand => _parent.JumpToMaxLinkCommand;
    public ICommand JumpToItemCommand => _parent.JumpToItemCommand;
    public ICommand SelectCommand => _parent.ItemClickedCommand;
    public List<SelectorComboBoxItem> WarriorItems => _parent.WarriorItems;
    public List<SelectorComboBoxItem> ItemItems => _parent.ItemItems;

    public string WarriorName => _baseWarriorService.IdToName(Warrior);

    public int Warrior
    {
        get => (int)_model.Warrior;
        set
        {
            if (SetProperty(_model.Warrior, (WarriorId)value, v => _model.Warrior = v))
            {
                RaisePropertyChanged(nameof(WarriorName));
                UpdateWarriorImage();
            }
        }
    }

    public int Strength => _strengthService.CalculateScenarioWarriorStrength(_scenario, _model);

    public WarriorClassId Class
    {
        get => _model.Class;
        set
        {
            var oldClass = _model.Class;
            if (SetProperty(_model.Class, value, v => _model.Class = v))
            {
                if (Class == WarriorClassId.ArmyLeader)
                {
                    _parent.UpdateLeaders(Army, this);
                }
                else if (oldClass == WarriorClassId.ArmyLeader)
                {
                    _parent.UpdateLeaders(Army, null);
                }
            }
        }
    }

    public void SetClassNoEvent(WarriorClassId newClass)
    {
        if (_model.Class != newClass)
        {
            _model.Class = newClass;
            RaisePropertyChanged(nameof(Class));
        }
    }

    public int Kingdom
    {
        get => (int)_model.Kingdom;
        set
        {
            if (SetProperty(_model.Kingdom, (KingdomId)value, v => _model.Kingdom = v))
            {
                RaisePropertyChanged(nameof(KingdomName));
            }
        }
    }

    public string KingdomName
    {
        get
        {
            if (_kingdomService.ValidateId(Kingdom))
            {
                return _kingdomService.IdToName(Kingdom);
            }
            return "Default";
        }
    }

    public int Army
    {
        get => _model.Army;
        set => SetProperty(_model.Army, value, v => _model.Army = v);
    }

    public int Item
    {
        get => (int)_model.Item;
        set => SetProperty(_model.Item, (ItemId)value, v => _model.Item = v);
    }

    public object? WarriorImage => _spriteProvider.GetSprite(SpriteType.StlBushouS, _warriorImageId);

    private int _warriorImageId;

    private void UpdateWarriorImage()
    {
        if (!_baseWarriorService.ValidateId(Warrior))
        {
            _warriorImageId = -1;
        }
        else
        {
            _warriorImageId = _baseWarriorService.Retrieve(Warrior).Sprite;
        }
        
        RaisePropertyChanged(nameof(WarriorImage));
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
                UpdateStrength();
            }
        }
    }

    public void UpdateStrength()
    {
        RaisePropertyChanged(nameof(Strength));
    }

    

    

    public SwPokemonSlotViewModel? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
            {
                if (!_suppressUpdateNested)
                {
                    value?.UpdateNested();
                }
            }
        }
    }

    public ObservableCollection<SwPokemonSlotViewModel> ScenarioPokemonSlots { get; } = new ObservableCollection<SwPokemonSlotViewModel>();
}
