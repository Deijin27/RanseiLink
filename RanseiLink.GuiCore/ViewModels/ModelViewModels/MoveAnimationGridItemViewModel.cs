#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.GuiCore.ViewModels;

public abstract class MoveAnimationViewModelBase : ViewModelBase
{
    protected readonly MoveAnimation _model;
    protected MoveAnimationId _id;

    public MoveAnimationViewModelBase(MoveAnimationId id, MoveAnimation model)
    {
        _id = id;
        _model = model;
    }
}

public class MoveAnimationGridItemViewModel : MoveAnimationViewModelBase
{
    public MoveAnimationGridItemViewModel(MoveAnimationId id, MoveAnimation model) : base(id, model)
    {
        Id = (int)id;
        Name = id.ToString();
    }

    public int Id { get; }
    public string Name { get; }

    public int UnknownA
    {
        get => _model.UnknownA;
        set => Set(_model.UnknownA, value, v => _model.UnknownA = value);
    }

    public int UnknownB
    {
        get => _model.UnknownB;
        set => Set(_model.UnknownB, value, v => _model.UnknownB = value);
    }
}
