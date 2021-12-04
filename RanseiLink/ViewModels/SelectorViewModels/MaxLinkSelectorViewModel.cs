using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Linq;

namespace RanseiLink.ViewModels;

public delegate MaxLinkSelectorViewModel MaxLinkSelectorViewModelFactory(IMaxLinkService service);

public class MaxLinkSelectorViewModel : SelectorViewModelBase<WarriorId, IMaxLink, MaxLinkViewModel>
{
    private readonly MaxLinkViewModelFactory _factory;

    public MaxLinkSelectorViewModel(IServiceContainer container, IMaxLinkService dataService)
        : base(container, dataService, EnumUtil.GetValuesExceptDefaults<WarriorId>().ToArray()) 
    { 
        _factory = container.Resolve<MaxLinkViewModelFactory>();
        Selected = WarriorId.PlayerMale_1;
    }

    protected override MaxLinkViewModel NewViewModel(IMaxLink model) => _factory(model);
}
