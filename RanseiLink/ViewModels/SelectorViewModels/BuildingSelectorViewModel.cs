using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public delegate BuildingSelectorViewModel BuildingSelectorViewModelFactory(IEditorContext context);

public class BuildingSelectorViewModel : SelectorViewModelBase<BuildingId, IBuilding, BuildingViewModel>
{
    private readonly BuildingViewModelFactory _factory;
    private readonly IEditorContext _context;

    public BuildingSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.Building) 
    {
        _context = context;
        _factory = container.Resolve<BuildingViewModelFactory>();
        Selected = BuildingId.Shop_0;
    }

    protected override BuildingViewModel NewViewModel(IBuilding model) => _factory(Selected, model, _context);
}
