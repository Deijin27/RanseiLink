using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Linq;

namespace RanseiLink.ViewModels;

public delegate BaseWarriorSelectorViewModel BaseWarriorSelectorViewModelFactory(IBaseWarriorService service);

public class BaseWarriorSelectorViewModel : SelectorViewModelBase<WarriorId, IBaseWarrior, BaseWarriorViewModel>
{
    private readonly BaseWarriorViewModelFactory _factory;

    public BaseWarriorSelectorViewModel(IServiceContainer container, IBaseWarriorService dataService)
        : base(container, dataService, EnumUtil.GetValuesExceptDefaults<WarriorId>().ToArray()) 
    { 
        _factory = container.Resolve<BaseWarriorViewModelFactory>();
        Selected = WarriorId.PlayerMale_1;
    }

    protected override BaseWarriorViewModel NewViewModel(IBaseWarrior model) => _factory(model);
}
