using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.ViewModels;

public delegate WarriorSkillSelectorViewModel WarriorSkillSelectorViewModelFactory(IWarriorSkillService service);

public class WarriorSkillSelectorViewModel : SelectorViewModelBase<WarriorSkillId, IWarriorSkill, WarriorSkillViewModel>
{
    private readonly WarriorSkillViewModelFactory _factory;

    public WarriorSkillSelectorViewModel(IServiceContainer container, IWarriorSkillService dataService)
        : base(container, dataService) 
    {
        _factory = container.Resolve<WarriorSkillViewModelFactory>();
        Selected = WarriorSkillId.Adrenaline;
    }

    protected override WarriorSkillViewModel NewViewModel(IWarriorSkill model) => _factory(model);
}
