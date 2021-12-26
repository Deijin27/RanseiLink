using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public delegate GimmickSelectorViewModel GimmickSelectorViewModelFactory(IEditorContext context);

public class GimmickSelectorViewModel : SelectorViewModelBase<GimmickId, IGimmick, GimmickViewModel>
{
    private readonly GimmickViewModelFactory _factory;
    private readonly IEditorContext _context;
    public GimmickSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.Gimmick) 
    { 
        _context = context;
        _factory = container.Resolve<GimmickViewModelFactory>();
        Selected = GimmickId.dummy_0;
    }

    protected override GimmickViewModel NewViewModel(IGimmick model) => _factory(Selected, model, _context);
}
