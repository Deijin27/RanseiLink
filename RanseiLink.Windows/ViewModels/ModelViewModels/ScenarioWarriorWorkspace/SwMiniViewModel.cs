﻿#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;

namespace RanseiLink.Windows.ViewModels;

public class SwMiniViewModel(
    IScenarioPokemonService scenarioPokemonService,
    IBaseWarriorService baseWarriorService,
    ICachedSpriteProvider spriteProvider,
    IKingdomService kingdomService,
    IStrengthService strengthService) : ViewModelBase
{
    public delegate SwMiniViewModel Factory();

    private ScenarioId _scenario;
    private ScenarioWarrior _model = new();
    private ScenarioWarriorWorkspaceViewModel _parent = null!;

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
        var spService = scenarioPokemonService.Retrieve((int)scenario);
        for (int i = 0; i < 8; i++)
        {
            var slotVm = new SwPokemonSlotViewModel(_parent.ScenarioPokemonItems, scenario, model, i, spVm, spService, spriteProvider);
            slotVm.PropertyChanged += SlotVm_PropertyChanged;
            ScenarioPokemonSlots.Add(slotVm);
        }
        SelectedItem = ScenarioPokemonSlots[0];

        _suppressUpdateNested = false;

        return this;
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

    public string WarriorName => baseWarriorService.IdToName(Warrior);

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

    public int Strength => strengthService.CalculateScenarioWarriorStrength(_scenario, _model);

    public WarriorClassId Class
    {
        get => _model.Class;
        set
        {
            var oldClass = _model.Class;
            if (SetProperty(_model.Class, value, v => _model.Class = v))
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
            if (kingdomService.ValidateId(Kingdom))
            {
                return kingdomService.IdToName(Kingdom);
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

    public object? WarriorImage => spriteProvider.GetSprite(SpriteType.StlBushouS, _warriorImageId);

    private int _warriorImageId;

    private void UpdateWarriorImage()
    {
        if (!baseWarriorService.ValidateId(Warrior))
        {
            _warriorImageId = -1;
        }
        else
        {
            _warriorImageId = baseWarriorService.Retrieve(Warrior).Sprite;
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
    private SwPokemonSlotViewModel? _selectedItem;
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
