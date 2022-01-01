using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Services;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public delegate KingdomViewModel KingdomViewModelFactory(KingdomId id, IKingdom model, IEditorContext context);

public abstract class KingdomViewModelBase : ViewModelBase
{
    private readonly IKingdom _model;

    public KingdomViewModelBase(KingdomId id, IKingdom model)
    {
        Id = id;
        _model = model;
    }

    public KingdomId Id { get; }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public KingdomId MapConnection0
    {
        get => _model.MapConnection0;
        set => RaiseAndSetIfChanged(_model.MapConnection0, value, v => _model.MapConnection0 = v);
    }
    public KingdomId MapConnection1
    {
        get => _model.MapConnection1;
        set => RaiseAndSetIfChanged(_model.MapConnection1, value, v => _model.MapConnection1 = v);
    }
    public KingdomId MapConnection2
    {
        get => _model.MapConnection2;
        set => RaiseAndSetIfChanged(_model.MapConnection2, value, v => _model.MapConnection2 = v);
    }
    public KingdomId MapConnection3
    {
        get => _model.MapConnection3;
        set => RaiseAndSetIfChanged(_model.MapConnection3, value, v => _model.MapConnection3 = v);
    }
    public KingdomId MapConnection4
    {
        get => _model.MapConnection4;
        set => RaiseAndSetIfChanged(_model.MapConnection4, value, v => _model.MapConnection4 = v);
    }
    public KingdomId MapConnection5
    {
        get => _model.MapConnection5;
        set => RaiseAndSetIfChanged(_model.MapConnection5, value, v => _model.MapConnection5 = v);
    }
    public KingdomId MapConnection6
    {
        get => _model.MapConnection6;
        set => RaiseAndSetIfChanged(_model.MapConnection6, value, v => _model.MapConnection6 = v);
    }
    public KingdomId MapConnection7
    {
        get => _model.MapConnection7;
        set => RaiseAndSetIfChanged(_model.MapConnection7, value, v => _model.MapConnection7 = v);
    }
    public KingdomId MapConnection8
    {
        get => _model.MapConnection8;
        set => RaiseAndSetIfChanged(_model.MapConnection8, value, v => _model.MapConnection8 = v);
    }
    public KingdomId MapConnection9
    {
        get => _model.MapConnection9;
        set => RaiseAndSetIfChanged(_model.MapConnection9, value, v => _model.MapConnection9 = v);
    }
    public KingdomId MapConnection10
    {
        get => _model.MapConnection10;
        set => RaiseAndSetIfChanged(_model.MapConnection10, value, v => _model.MapConnection10 = v);
    }
    public KingdomId MapConnection11
    {
        get => _model.MapConnection11;
        set => RaiseAndSetIfChanged(_model.MapConnection11, value, v => _model.MapConnection11 = v);
    }
    public KingdomId MapConnection12
    {
        get => _model.MapConnection12;
        set => RaiseAndSetIfChanged(_model.MapConnection12, value, v => _model.MapConnection12 = v);
    }
    public BattleConfigId BattleConfig
    {
        get => _model.BattleConfig;
        set => RaiseAndSetIfChanged(_model.BattleConfig, value, v => _model.BattleConfig = v);
    }

    public uint Unknown1
    {
        get => _model.Unknown_R2_C24_L3;
        set => RaiseAndSetIfChanged(_model.Unknown_R2_C24_L3, value, v => _model.Unknown_R2_C24_L3 = v);
    }

    public uint Unknown2
    {
        get => _model.Unknown_R5_C22_L4;
        set => RaiseAndSetIfChanged(_model.Unknown_R5_C22_L4, value, v => _model.Unknown_R5_C22_L4 = v);
    }

    public uint Unknown3
    {
        get => _model.Unknown_R5_C26_L4;
        set => RaiseAndSetIfChanged(_model.Unknown_R5_C26_L4, value, v => _model.Unknown_R5_C26_L4 = v);
    }
}

public class KingdomViewModel : KingdomViewModelBase
{
    public KingdomViewModel(KingdomId id, IKingdom model, IEditorContext context) : base(id, model) 
    {
        var jumpService = context.JumpService;

        JumpToBattleConfigCommand = new RelayCommand<BattleConfigId>(jumpService.JumpToBattleConfig);
    }

    public ICommand JumpToBattleConfigCommand { get; }

}