using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System.Collections.Generic;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public interface IBuildingViewModel
{
    void SetModel(BuildingId id, Building model);
}

public class BuildingViewModel : ViewModelBase, IBuildingViewModel
{
    private Building _model;

    public BuildingViewModel(IJumpService jumpService, IIdToNameService idToNameService)
    {
        _model = new Building();

        JumpToBattleConfigCommand = new RelayCommand<BattleConfigId>(id => jumpService.JumpTo(BattleConfigSelectorEditorModule.Id, (int)id));

        KingdomItems = idToNameService.GetComboBoxItemsExceptDefault<IKingdomService>();
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

    public int Kingdom
    {
        get => (int)_model.Kingdom;
        set => RaiseAndSetIfChanged(_model.Kingdom, (KingdomId)value, v => _model.Kingdom = v);
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
