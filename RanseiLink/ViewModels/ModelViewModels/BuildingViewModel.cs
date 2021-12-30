using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.ViewModels;

public delegate BuildingViewModel BuildingViewModelFactory(BuildingId id, IBuilding model);

public abstract class BuildingViewModelBase : ViewModelBase
{
    private readonly IBuilding _model;

    public BuildingViewModelBase(BuildingId id, IBuilding model)
    {
        Id = id;
        _model = model;
    }

    public BuildingId Id { get; }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public KingdomId Kingdom
    {
        get => _model.Kingdom;
        set => RaiseAndSetIfChanged(_model.Kingdom, value, v => _model.Kingdom = v);
    }

    public BattleMapId BattleMap1
    {
        get => _model.BattleMap1;
        set => RaiseAndSetIfChanged(_model.BattleMap1, value, v => _model.BattleMap1 = v);
    }

    public BattleMapId BattleMap2
    {
        get => _model.BattleMap2;
        set => RaiseAndSetIfChanged(_model.BattleMap2, value, v => _model.BattleMap2 = v);
    }

    public BattleMapId BattleMap3
    {
        get => _model.BattleMap3;
        set => RaiseAndSetIfChanged(_model.BattleMap3, value, v => _model.BattleMap3 = v);
    }

    public BuildingSpriteId Sprite1
    {
        get => _model.Sprite1;
        set => RaiseAndSetIfChanged(_model.Sprite1, value, v => _model.Sprite1 = v);
    }

    public BuildingSpriteId Sprite2
    {
        get => _model.Sprite2;
        set => RaiseAndSetIfChanged(_model.Sprite2, value, v => _model.Sprite2 = v);
    }

    public BuildingSpriteId Sprite3
    {
        get => _model.Sprite3;
        set => RaiseAndSetIfChanged(_model.Sprite3, value, v => _model.Sprite3 = v);
    }
}

public class BuildingViewModel : BuildingViewModelBase
{
    public BuildingViewModel(BuildingId id, IBuilding model) : base(id, model) { }
}

public class BuildingGridItemViewModel : BuildingViewModelBase
{
    public BuildingGridItemViewModel(BuildingId id, IBuilding model) : base(id, model) { }
}