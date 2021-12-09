using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Linq;

namespace RanseiLink.ViewModels;

public delegate BaseWarriorSelectorViewModel BaseWarriorSelectorViewModelFactory(IEditorContext context);

public class BaseWarriorSelectorViewModel : SelectorViewModelBase<WarriorId, IBaseWarrior, BaseWarriorViewModel>
{
    private readonly BaseWarriorViewModelFactory _factory;

    public BaseWarriorSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.BaseWarrior, EnumUtil.GetValuesExceptDefaults<WarriorId>().ToArray()) 
    { 
        _factory = container.Resolve<BaseWarriorViewModelFactory>();
        Selected = WarriorId.PlayerMale_1;
    }

    protected override BaseWarriorViewModel NewViewModel(IBaseWarrior model) => _factory(model);
}
