#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Windows.Services;
using System.Collections.Generic;
using System.Windows.Input;

namespace RanseiLink.Windows.ViewModels;

public class KingdomViewModel : ViewModelBase
{
    private Kingdom _model;

    public KingdomViewModel(IJumpService jumpService, IIdToNameService idToNameService)
    {
        _model = new Kingdom();
        JumpToBattleConfigCommand = new RelayCommand<BattleConfigId>(id => jumpService.JumpTo(BattleConfigSelectorEditorModule.Id, (int)id));

        KingdomItems = idToNameService.GetComboBoxItemsPlusDefault<IKingdomService>();
    }

    public void SetModel(KingdomId id, Kingdom model)
    {
        Id = id;
        _model = model;
        RaiseAllPropertiesChanged();
    }

    public KingdomId Id { get; private set; }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public List<SelectorComboBoxItem> KingdomItems { get; }

    public int MapConnection0
    {
        get => (int)_model.MapConnection0;
        set => RaiseAndSetIfChanged(_model.MapConnection0, (KingdomId)value, v => _model.MapConnection0 = v);
    }
    public int MapConnection1
    {
        get => (int)_model.MapConnection1;
        set => RaiseAndSetIfChanged(_model.MapConnection1, (KingdomId)value, v => _model.MapConnection1 = v);
    }
    public int MapConnection2
    {
        get => (int)_model.MapConnection2;
        set => RaiseAndSetIfChanged(_model.MapConnection2, (KingdomId)value, v => _model.MapConnection2 = v);
    }
    public int MapConnection3
    {
        get => (int)_model.MapConnection3;
        set => RaiseAndSetIfChanged(_model.MapConnection3, (KingdomId)value, v => _model.MapConnection3 = v);
    }
    public int MapConnection4
    {
        get => (int)_model.MapConnection4;
        set => RaiseAndSetIfChanged(_model.MapConnection4, (KingdomId)value, v => _model.MapConnection4 = v);
    }
    public int MapConnection5
    {
        get => (int)_model.MapConnection5;
        set => RaiseAndSetIfChanged(_model.MapConnection5, (KingdomId)value, v => _model.MapConnection5 = v);
    }
    public int MapConnection6
    {
        get => (int)_model.MapConnection6;
        set => RaiseAndSetIfChanged(_model.MapConnection6, (KingdomId)value, v => _model.MapConnection6 = v);
    }
    public int MapConnection7
    {
        get => (int)_model.MapConnection7;
        set => RaiseAndSetIfChanged(_model.MapConnection7, (KingdomId)value, v => _model.MapConnection7 = v);
    }
    public int MapConnection8
    {
        get => (int)_model.MapConnection8;
        set => RaiseAndSetIfChanged(_model.MapConnection8, (KingdomId)value, v => _model.MapConnection8 = v);
    }
    public int MapConnection9
    {
        get => (int)_model.MapConnection9;
        set => RaiseAndSetIfChanged(_model.MapConnection9, (KingdomId)value, v => _model.MapConnection9 = v);
    }
    public int MapConnection10
    {
        get => (int)_model.MapConnection10;
        set => RaiseAndSetIfChanged(_model.MapConnection10, (KingdomId)value, v => _model.MapConnection10 = v);
    }
    public int MapConnection11
    {
        get => (int)_model.MapConnection11;
        set => RaiseAndSetIfChanged(_model.MapConnection11, (KingdomId)value, v => _model.MapConnection11 = v);
    }
    public int MapConnection12
    {
        get => (int)_model.MapConnection12;
        set => RaiseAndSetIfChanged(_model.MapConnection12, (KingdomId)value, v => _model.MapConnection12 = v);
    }
    public BattleConfigId BattleConfig
    {
        get => _model.BattleConfig;
        set => RaiseAndSetIfChanged(_model.BattleConfig, value, v => _model.BattleConfig = v);
    }

    public int Unknown1
    {
        get => _model.Unknown_R2_C24_L3;
        set => RaiseAndSetIfChanged(_model.Unknown_R2_C24_L3, value, v => _model.Unknown_R2_C24_L3 = v);
    }

    public int Unknown2
    {
        get => _model.Unknown_R5_C22_L4;
        set => RaiseAndSetIfChanged(_model.Unknown_R5_C22_L4, value, v => _model.Unknown_R5_C22_L4 = v);
    }

    public int Unknown3
    {
        get => _model.Unknown_R5_C26_L4;
        set => RaiseAndSetIfChanged(_model.Unknown_R5_C26_L4, value, v => _model.Unknown_R5_C26_L4 = v);
    }

    public ICommand JumpToBattleConfigCommand { get; }
}
