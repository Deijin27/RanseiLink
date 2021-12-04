using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.ViewModels;

public delegate MoveRangeSelectorViewModel MoveRangeSelectorViewModelFactory(IMoveRangeService service);

public class MoveRangeSelectorViewModel : SelectorViewModelBase<MoveRangeId, IMoveRange, MoveRangeViewModel>
{
    private readonly MoveRangeViewModelFactory _factory;

    public MoveRangeSelectorViewModel(IServiceContainer container, IMoveRangeService dataService)
        : base(container, dataService) 
    {
        _factory = container.Resolve<MoveRangeViewModelFactory>();
        Selected = MoveRangeId.Ahead1Tile;
    }

    protected override MoveRangeViewModel NewViewModel(IMoveRange model) => _factory(model);
}
