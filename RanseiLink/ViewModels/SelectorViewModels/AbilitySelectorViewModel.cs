using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Linq;

namespace RanseiLink.ViewModels;

public delegate AbilitySelectorViewModel AbilitySelectorViewModelFactory(IEditorContext context);

public class AbilitySelectorViewModel : SelectorViewModelBase<AbilityId, IAbility, AbilityViewModel>
{
    private readonly AbilityViewModelFactory _factory;

    public AbilitySelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.Ability, EnumUtil.GetValuesExceptDefaults<AbilityId>().ToArray()) 
    {
        _factory = container.Resolve<AbilityViewModelFactory>();
        Selected = AbilityId.Levitate;
    }

    protected override AbilityViewModel NewViewModel(IAbility model) => _factory(model);
}
