#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System.Collections.Generic;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class BuildingViewModel : ViewModelBase
{
    private Building _model;

    public BuildingViewModel(IJumpService jumpService, IIdToNameService idToNameService)
    {
        _model = new Building();

        JumpToBattleConfigCommand = new RelayCommand<BattleConfigId>(id => jumpService.JumpTo(BattleConfigSelectorEditorModule.Id, (int)id));

        KingdomItems = idToNameService.GetComboBoxItemsExceptDefault<IKingdomService>();
        BuildingItems = idToNameService.GetComboBoxItemsPlusDefault<IBuildingService>();
    }

    public void SetModel(BuildingId id, Building model)
    {
        Id = id;
        _model = model;
        RaiseAllPropertiesChanged();
    }

    public BuildingId Id { get; private set; }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public List<SelectorComboBoxItem> KingdomItems { get; }
    public List<SelectorComboBoxItem> BuildingItems { get; }

    public int Kingdom
    {
        get => (int)_model.Kingdom;
        set => RaiseAndSetIfChanged(_model.Kingdom, (KingdomId)value, v => _model.Kingdom = v);
    }

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

    public ICommand JumpToBattleConfigCommand { get; }
}
