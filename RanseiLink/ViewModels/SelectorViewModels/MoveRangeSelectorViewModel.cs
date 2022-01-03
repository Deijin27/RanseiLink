using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public delegate MoveRangeSelectorViewModel MoveRangeSelectorViewModelFactory(IEditorContext context);

public class MoveRangeSelectorViewModel : SelectorViewModelBase<MoveRangeId, IMoveRange, MoveRangeViewModel>
{
    private readonly MoveRangeViewModelFactory _factory;

    public MoveRangeSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.MoveRange) 
    {
        _factory = container.Resolve<MoveRangeViewModelFactory>();
        Selected = MoveRangeId.Ahead1Tile;
    }

    protected override MoveRangeViewModel NewViewModel(IMoveRange model) => _factory(model);
}
