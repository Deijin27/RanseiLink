using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public abstract class MoveAnimationViewModelBase : ViewModelBase
{
    protected readonly IMoveAnimation _model;
    protected MoveAnimationId _id;

    public MoveAnimationViewModelBase(MoveAnimationId id, IMoveAnimation model)
    {
        _id = id;
        _model = model;
    }
}

public class MoveAnimationGridItemViewModel : MoveAnimationViewModelBase
{
    public MoveAnimationGridItemViewModel(MoveAnimationId id, IMoveAnimation model) : base(id, model)
    {
        Id = (uint)id;
        Name = id.ToString();
    }

    public uint Id { get; }
    public string Name { get; }

    public uint UnknownA
    {
        get => _model.UnknownA;
        set => RaiseAndSetIfChanged(_model.UnknownA, value, v => _model.UnknownA = value);
    }

    public uint UnknownB
    {
        get => _model.UnknownB;
        set => RaiseAndSetIfChanged(_model.UnknownB, value, v => _model.UnknownB = value);
    }
}
