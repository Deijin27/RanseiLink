using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Windows.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

namespace RanseiLink.Windows.ViewModels;

public class SwMiniViewModel : ViewModelBase
{
    public delegate SwMiniViewModel Factory();

    private ScenarioId _scenario;
    private ScenarioWarrior _model;
    private readonly IScenarioPokemonService _scenarioPokemonService;
    private readonly ICachedSpriteProvider _spriteProvider;
    private readonly IBaseWarriorService _baseWarriorService;
    private readonly IKingdomService _kingdomService;
    private readonly IStrengthService _strengthService;
    private ScenarioWarriorWorkspaceViewModel _parent;

    public SwMiniViewModel(
        IScenarioPokemonService scenarioPokemonService,
        IBaseWarriorService baseWarriorService,
        ICachedSpriteProvider spriteProvider,
        IKingdomService kingdomService,
        IStrengthService strengthService)
    {
        _kingdomService = kingdomService;
        _scenarioPokemonService = scenarioPokemonService;
        _spriteProvider = spriteProvider;
        _baseWarriorService = baseWarriorService;
        _strengthService = strengthService;
    }

    public SwMiniViewModel Init(
        int id,
        ScenarioWarrior model, 
        ScenarioId scenario, 
        ScenarioWarriorWorkspaceViewModel parent, 
        ScenarioPokemonViewModel spVm)
    {
        Id = id;
        _model = model;
        _parent = parent;
        _scenario = scenario;
        _suppressUpdateNested = true;

        UpdateWarriorImage();

        ScenarioPokemonSlots.Clear();
        var spService = _scenarioPokemonService.Retrieve((int)scenario);
        for (int i = 0; i < 8; i++)
        {
            var slotVm = new SwPokemonSlotViewModel(_parent.ScenarioPokemonItems, scenario, model, i, spVm, spService, _spriteProvider);
            slotVm.PropertyChanged += SlotVm_PropertyChanged;
            ScenarioPokemonSlots.Add(slotVm);
        }
        SelectedItem = ScenarioPokemonSlots[0];

        _suppressUpdateNested = false;

        return this;
    }

    public int Id { get; private set; }

    private void SlotVm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SwPokemonSlotViewModel.ScenarioPokemonId))
        {
            var slotVm = (SwPokemonSlotViewModel)sender;
            UpdateStrength();
            _parent.UpdateScenarioPokemonComboItemName(slotVm.ScenarioPokemonId);
        }
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
            if (RaiseAndSetIfChanged(_model.Warrior, (WarriorId)value, v => _model.Warrior = v))
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
        set
        {
            if (RaiseAndSetIfChanged(_model.Kingdom, (KingdomId)value, v => _model.Kingdom = v))
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
        set => RaiseAndSetIfChanged(_model.Army, value, v => _model.Army = v);
    }

    public int Item
    {
        get => (int)_model.Item;
        set => RaiseAndSetIfChanged(_model.Item, (ItemId)value, v => _model.Item = v);
    }

    public ImageSource WarriorImage => _spriteProvider.GetSprite(SpriteType.StlBushouS, _warriorImageId);

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
