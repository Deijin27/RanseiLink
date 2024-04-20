using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.ViewModels;

public class BuildingViewModel : ViewModelBase
{
    public delegate BuildingViewModel Factory();

    private readonly BuildingWorkspaceViewModel _parent;
    private readonly IKingdomService _kingdomService;
    private readonly ICachedSpriteProvider _cachedSpriteProvider;
    private readonly Building _model;

    public Building Model => _model;

    public BuildingViewModel(ScenarioBuildingViewModel sbvm, BuildingWorkspaceViewModel parent, IKingdomService kingdomService, ICachedSpriteProvider cachedSpriteProvider, BuildingId id, Building model)
    {
        ScenarioBuildingVm = sbvm;   
        _parent = parent;
        _kingdomService = kingdomService;
        _cachedSpriteProvider = cachedSpriteProvider;
        Id = (int)id;
        _model = model;
    }

    public ScenarioBuildingViewModel ScenarioBuildingVm { get; }

    public int Id { get; private set; }

    public string Name
    {
        get => _model.Name;
        set => Set(_model.Name, value, v => _model.Name = v);
    }

    public List<SelectorComboBoxItem> BuildingItems => _parent.BuildingItems;

    public int Slot { get; set; }

    public int Kingdom
    {
        get => (int)_model.Kingdom;
        set
        {
            if (Set(_model.Kingdom, (KingdomId)value, v => _model.Kingdom = v))
            {
                Notify(nameof(KingdomName));
            }
        }
    }

    public string KingdomName => _kingdomService.IdToName(Kingdom);

    public int Building1
    {
        get => (int)_model.Building1;
        set => Set(_model.Building1, (BuildingId)value, v => _model.Building1 = v);
    }

    public int Building2
    {
        get => (int)_model.Building2;
        set => Set(_model.Building2, (BuildingId)value, v => _model.Building2 = v);
    }

    public int Building3
    {
        get => (int)_model.Building3;
        set => Set(_model.Building3, (BuildingId)value, v => _model.Building3 = v);
    }

    public int Building4
    {
        get => (int)_model.Building4;
        set => Set(_model.Building4, (BuildingId)value, v => _model.Building4 = v);
    }

    public int Building5
    {
        get => (int)_model.Building5;
        set => Set(_model.Building5, (BuildingId)value, v => _model.Building5 = v);
    }

    public int Building6
    {
        get => (int)_model.Building6;
        set => Set(_model.Building6, (BuildingId)value, v => _model.Building6 = v);
    }

    public int Building7
    {
        get => (int)_model.Building7;
        set => Set(_model.Building7, (BuildingId)value, v => _model.Building7 = v);
    }

    public int Building8
    {
        get => (int)_model.Building8;
        set => Set(_model.Building8, (BuildingId)value, v => _model.Building8 = v);
    }

    public BattleConfigId BattleConfig1
    {
        get => _model.BattleConfig1;
        set => Set(_model.BattleConfig1, value, v => _model.BattleConfig1 = v);
    }

    public BattleConfigId BattleConfig2
    {
        get => _model.BattleConfig2;
        set => Set(_model.BattleConfig2, value, v => _model.BattleConfig2 = v);
    }

    public BattleConfigId BattleConfig3
    {
        get => _model.BattleConfig3;
        set => Set(_model.BattleConfig3, value, v => _model.BattleConfig3 = v);
    }

    public int Sprite1
    {
        get => _model.Sprite1;
        set
        {
            if (Set(_model.Sprite1, value, v => _model.Sprite1 = v))
            {
                Notify(nameof(Sprite1Image));
            }
        }
    }

    public object? Sprite1Image => _cachedSpriteProvider.GetSprite(Core.Services.SpriteType.IconInstS, (int)Sprite1);

    public int Sprite2
    {
        get => _model.Sprite2;
        set
        {
            if (Set(_model.Sprite2, value, v => _model.Sprite2 = v))
            {
                Notify(nameof(Sprite2Image)); 
            }
        }
    }

    public object? Sprite2Image => _cachedSpriteProvider.GetSprite(Core.Services.SpriteType.IconInstS, (int)Sprite2);

    public int Sprite3
    {
        get => _model.Sprite3;
        set
        {
            if (Set(_model.Sprite3, value, v => _model.Sprite3 = v))
            {
                Notify(nameof(Sprite3Image));
            }
        }
    }

    public object? Sprite3Image => _cachedSpriteProvider.GetSprite(Core.Services.SpriteType.IconInstS, (int)Sprite3);

    public BuildingFunctionId Function
    {
        get => _model.Function;
        set => Set(_model.Function, value, v => _model.Function = v);
    }

    public ICommand JumpToBattleConfigCommand => _parent.JumpToBattleConfigCommand;

    public ICommand SelectCommand => _parent.ItemClickedCommand;
}
