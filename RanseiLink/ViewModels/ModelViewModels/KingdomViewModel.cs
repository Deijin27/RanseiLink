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
}

public class KingdomViewModel : KingdomViewModelBase
{
    private readonly ICachedMsgBlockService _msgService;
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