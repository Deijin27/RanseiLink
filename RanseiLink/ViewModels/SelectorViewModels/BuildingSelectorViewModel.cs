using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public delegate BuildingSelectorViewModel BuildingSelectorViewModelFactory(IEditorContext context);

public class BuildingSelectorViewModel : SelectorViewModelBase<BuildingId, IBuilding, BuildingViewModel>
{
    private readonly BuildingViewModelFactory _factory;

    public BuildingSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.Building) 
    {
        _factory = container.Resolve<BuildingViewModelFactory>();
        Selected = BuildingId.Shop_0;
    }

    protected override BuildingViewModel NewViewModel(IBuilding model) => _factory(model);
}
