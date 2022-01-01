using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Services;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public delegate BuildingViewModel BuildingViewModelFactory(BuildingId id, IBuilding model, IEditorContext context);

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

    public BuildingFunctionId Function
    {
        get => _model.Function;
        set => RaiseAndSetIfChanged(_model.Function, value, v => _model.Function = v);
    }
}

public class BuildingViewModel : BuildingViewModelBase
{
    public BuildingViewModel(BuildingId id, IBuilding model, IEditorContext context) : base(id, model) 
    {
        var jumpService = context.JumpService;

        JumpToBattleConfigCommand = new RelayCommand<BattleConfigId>(jumpService.JumpToBattleConfig);
    }

    public ICommand JumpToBattleConfigCommand { get; }
}

public class BuildingGridItemViewModel : BuildingViewModelBase
{
    public BuildingGridItemViewModel(BuildingId id, IBuilding model) : base(id, model) { }
}