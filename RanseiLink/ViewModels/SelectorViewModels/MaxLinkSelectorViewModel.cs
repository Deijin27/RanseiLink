using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using System.Linq;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public delegate MaxLinkSelectorViewModel MaxLinkSelectorViewModelFactory(IEditorContext context);

public class MaxLinkSelectorViewModel : SelectorViewModelBase<WarriorId, IMaxLink, MaxLinkViewModel>
{
    private readonly MaxLinkViewModelFactory _factory;

    public MaxLinkSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.MaxLink, EnumUtil.GetValuesExceptDefaults<WarriorId>().ToArray()) 
    { 
        _factory = container.Resolve<MaxLinkViewModelFactory>();
        Selected = WarriorId.PlayerMale_1;
    }

    protected override MaxLinkViewModel NewViewModel(IMaxLink model) => _factory(model);
}
