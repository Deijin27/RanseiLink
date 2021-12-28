using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Services;

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
    public BattleMapId BattleMap
    {
        get => _model.BattleMap;
        set => RaiseAndSetIfChanged(_model.BattleMap, value, v => _model.BattleMap = v);
    }
}

public class KingdomViewModel : KingdomViewModelBase
{
    //private readonly ICachedMsgBlockService _msgService;
    public KingdomViewModel(KingdomId id, IKingdom model, IEditorContext context) : base(id, model) 
    {
        //_msgService = context.CachedMsgBlockService;
    }

    //public string Description
    //{
    //    get => _msgService.GetKingdomDescription(Id);
    //    set => _msgService.SetKingdomDescription(Id, value);
    //}

}