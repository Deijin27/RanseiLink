#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public class BuildingSimpleKingdomMiniViewModel : ViewModelBase
{
    private readonly KingdomId _kingdom;
    protected readonly ICachedSpriteProvider _spriteProvider;
    public BuildingSimpleKingdomMiniViewModel(ICachedSpriteProvider spriteProvider, KingdomId kingdom)
    {
        _spriteProvider = spriteProvider;
        _kingdom = kingdom;
    }

    public object? KingdomImage => _spriteProvider.GetSprite(SpriteType.StlCastleIcon, (int)_kingdom);

    public KingdomId Kingdom => _kingdom;
}

public class BuildingWorkspaceViewModel : ViewModelBase
{
    private readonly IBuildingService _buildingService;
    private readonly ICachedSpriteProvider _cachedSpriteProvider;

    public ObservableCollection<object> Items { get; } = [];

    public void Deactivate()
    {
        _buildingService.Save();
    }

    public BuildingWorkspaceViewModel(
        IBuildingService buildingService, 
        IKingdomService kingdomService,
        IJumpService jumpService, 
        IIdToNameService idToNameService, 
        ICachedSpriteProvider cachedSpriteProvider,
        IAnimGuiManager animManager)
    {
        BuildingItems = idToNameService.GetComboBoxItemsPlusDefault<IBuildingService>();
        JumpToBattleConfigCommand = new RelayCommand<BattleConfigId>(id => jumpService.JumpTo(BattleConfigSelectorEditorModule.Id, (int)id));
        _buildingService = buildingService;
        _cachedSpriteProvider = cachedSpriteProvider;
        ItemClickedCommand = new RelayCommand<object>(ItemClicked);
        // load the building view models
        var vms = new List<BuildingViewModel>();
        foreach (var id in buildingService.ValidIds())
        {
            var model = buildingService.Retrieve(id);
            var vm = new BuildingViewModel(this, kingdomService, cachedSpriteProvider, (BuildingId)id, model);
            vms.Add(vm);
        }

        // put the view models into the list. maybe this won't be a list long term
        foreach (var kingdom in EnumUtil.GetValuesExceptDefaults<KingdomId>())
        {
            Items.Add(new BuildingSimpleKingdomMiniViewModel(cachedSpriteProvider, kingdom));
            var intKingdom = (int)kingdom;
            foreach (var vm in vms.Where(x => x.Kingdom == intKingdom))
            {
                Items.Add(vm);
            }
        }
        _selectedItem = vms.First();

        IconAnimVm = new(animManager, AnimationTypeId.IconInst, () => SelectedAnimation);
    }

    public List<SelectorComboBoxItem> BuildingItems { get; }

    public ICommand JumpToBattleConfigCommand { get; }

    public ICommand ItemClickedCommand { get; }

    private object _selectedItem;
    public object SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (RaiseAndSetIfChanged(ref _selectedItem, value))
            {
            }
        }
    }

    private void ItemClicked(object? sender)
    {
        if (sender is BuildingViewModel)
        {
            SelectedItem = sender;
        }
    }


    public AnimationViewModel? IconAnimVm { get; private set; }


    private int _selectedAnimation;
    public int SelectedAnimation
    {
        get => _selectedAnimation;
        set
        {
            if (RaiseAndSetIfChanged(_selectedAnimation, value, v => _selectedAnimation = v))
            {
                RaisePropertyChanged(nameof(SelectedAnimationImage));
                IconAnimVm?.OnIdChanged();
            }
        }
    }

    public object? SelectedAnimationImage => _cachedSpriteProvider.GetSprite(SpriteType.IconInstS, SelectedAnimation);
}



public class BuildingViewModel : ViewModelBase
{
    public delegate BuildingViewModel Factory();

    private readonly BuildingWorkspaceViewModel _parent;
    private readonly IKingdomService _kingdomService;
    private readonly ICachedSpriteProvider _cachedSpriteProvider;
    private Building _model = new();

    public BuildingViewModel(BuildingWorkspaceViewModel parent, IKingdomService kingdomService, ICachedSpriteProvider cachedSpriteProvider, BuildingId id, Building model)
    {
        _parent = parent;
        _kingdomService = kingdomService;
        _cachedSpriteProvider = cachedSpriteProvider;
        Id = (int)id;
        _model = model;
    }

    public int Id { get; private set; }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public List<SelectorComboBoxItem> BuildingItems => _parent.BuildingItems;

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

    public string KingdomName => _kingdomService.IdToName(Kingdom);

    public int Building1
    {
        get => (int)_model.Building1;
        set => RaiseAndSetIfChanged(_model.Building1, (BuildingId)value, v => _model.Building1 = v);
    }

    public int Building2
    {
        get => (int)_model.Building2;
        set => RaiseAndSetIfChanged(_model.Building2, (BuildingId)value, v => _model.Building2 = v);
    }

    public int Building3
    {
        get => (int)_model.Building3;
        set => RaiseAndSetIfChanged(_model.Building3, (BuildingId)value, v => _model.Building3 = v);
    }

    public int Building4
    {
        get => (int)_model.Building4;
        set => RaiseAndSetIfChanged(_model.Building4, (BuildingId)value, v => _model.Building4 = v);
    }

    public int Building5
    {
        get => (int)_model.Building5;
        set => RaiseAndSetIfChanged(_model.Building5, (BuildingId)value, v => _model.Building5 = v);
    }

    public int Building6
    {
        get => (int)_model.Building6;
        set => RaiseAndSetIfChanged(_model.Building6, (BuildingId)value, v => _model.Building6 = v);
    }

    public int Building7
    {
        get => (int)_model.Building7;
        set => RaiseAndSetIfChanged(_model.Building7, (BuildingId)value, v => _model.Building7 = v);
    }

    public int Building8
    {
        get => (int)_model.Building8;
        set => RaiseAndSetIfChanged(_model.Building8, (BuildingId)value, v => _model.Building8 = v);
    }

    public BattleConfigId BattleConfig1
    {
        get => _model.BattleConfig1;
        set => RaiseAndSetIfChanged(_model.BattleConfig1, value, v => _model.BattleConfig1 = v);
    }

    public BattleConfigId BattleConfig2
    {
        get => _model.BattleConfig2;
        set => RaiseAndSetIfChanged(_model.BattleConfig2, value, v => _model.BattleConfig2 = v);
    }

    public BattleConfigId BattleConfig3
    {
        get => _model.BattleConfig3;
        set => RaiseAndSetIfChanged(_model.BattleConfig3, value, v => _model.BattleConfig3 = v);
    }

    public int Sprite1
    {
        get => _model.Sprite1;
        set
        {
            if (RaiseAndSetIfChanged(_model.Sprite1, value, v => _model.Sprite1 = v))
            {
                RaisePropertyChanged(nameof(Sprite1Image));
            }
        }
    }

    public object? Sprite1Image => _cachedSpriteProvider.GetSprite(Core.Services.SpriteType.IconInstS, (int)Sprite1);

    public int Sprite2
    {
        get => _model.Sprite2;
        set
        {
            if (RaiseAndSetIfChanged(_model.Sprite2, value, v => _model.Sprite2 = v))
            {
                RaisePropertyChanged(nameof(Sprite2Image)); 
            }
        }
    }

    public object? Sprite2Image => _cachedSpriteProvider.GetSprite(Core.Services.SpriteType.IconInstS, (int)Sprite2);

    public int Sprite3
    {
        get => _model.Sprite3;
        set
        {
            if (RaiseAndSetIfChanged(_model.Sprite3, value, v => _model.Sprite3 = v))
            {
                RaisePropertyChanged(nameof(Sprite3Image));
            }
        }
    }

    public object? Sprite3Image => _cachedSpriteProvider.GetSprite(Core.Services.SpriteType.IconInstS, (int)Sprite3);

    public BuildingFunctionId Function
    {
        get => _model.Function;
        set => RaiseAndSetIfChanged(_model.Function, value, v => _model.Function = v);
    }

    public ICommand JumpToBattleConfigCommand => _parent.JumpToBattleConfigCommand;

    public ICommand SelectCommand => _parent.ItemClickedCommand;
}
