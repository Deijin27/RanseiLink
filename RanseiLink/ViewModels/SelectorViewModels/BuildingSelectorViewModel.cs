using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.ViewModels;

public delegate BuildingSelectorViewModel BuildingSelectorViewModelFactory(IBuildingService service);

public class BuildingSelectorViewModel : SelectorViewModelBase<BuildingId, IBuilding, BuildingViewModel>
{
    private readonly BuildingViewModelFactory _factory;

    public BuildingSelectorViewModel(IServiceContainer container, IBuildingService dataService)
        : base(container, dataService) 
    {
        _factory = container.Resolve<BuildingViewModelFactory>();
        Selected = BuildingId.Shop_0;
    }

    protected override BuildingViewModel NewViewModel(IBuilding model) => _factory(model);
}
