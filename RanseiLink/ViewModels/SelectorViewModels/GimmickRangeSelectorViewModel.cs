using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public class GimmickRangeSelectorViewModel : SelectorViewModelBase<GimmickRangeId, IMoveRange, MoveRangeViewModel>
{
    private readonly MoveRangeViewModelFactory _factory;

    public GimmickRangeSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.GimmickRange) 
    {
        _factory = container.Resolve<MoveRangeViewModelFactory>();
        Selected = GimmickRangeId.NoRange;
    }

    protected override MoveRangeViewModel NewViewModel(IMoveRange model) => _factory(model);
}
