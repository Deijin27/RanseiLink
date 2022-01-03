using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public class GimmickRangeSelectorViewModel : SelectorViewModelBase<GimmickRangeId, IAttackRange, AttackRangeViewModel>
{
    private readonly AttackRangeViewModelFactory _factory;

    public GimmickRangeSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.GimmickRange) 
    {
        _factory = container.Resolve<AttackRangeViewModelFactory>();
        Selected = GimmickRangeId.NoRange;
    }

    protected override AttackRangeViewModel NewViewModel(IAttackRange model) => _factory(model);
}
