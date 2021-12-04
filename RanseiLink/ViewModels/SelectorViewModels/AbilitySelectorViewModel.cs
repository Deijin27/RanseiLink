using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Linq;

namespace RanseiLink.ViewModels;

public delegate AbilitySelectorViewModel AbilitySelectorViewModelFactory(IAbilityService service);

public class AbilitySelectorViewModel : SelectorViewModelBase<AbilityId, IAbility, AbilityViewModel>
{
    private readonly AbilityViewModelFactory _factory;

    public AbilitySelectorViewModel(IServiceContainer container, IAbilityService dataService)
        : base(container, dataService, EnumUtil.GetValuesExceptDefaults<AbilityId>().ToArray()) 
    {
        _factory = container.Resolve<AbilityViewModelFactory>();
        Selected = AbilityId.Levitate;
    }

    protected override AbilityViewModel NewViewModel(IAbility model) => _factory(model);
}
