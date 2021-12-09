using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public delegate WarriorSkillSelectorViewModel WarriorSkillSelectorViewModelFactory(IEditorContext context);

public class WarriorSkillSelectorViewModel : SelectorViewModelBase<WarriorSkillId, IWarriorSkill, WarriorSkillViewModel>
{
    private readonly WarriorSkillViewModelFactory _factory;

    public WarriorSkillSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.WarriorSkill) 
    {
        _factory = container.Resolve<WarriorSkillViewModelFactory>();
        Selected = WarriorSkillId.Adrenaline;
    }

    protected override WarriorSkillViewModel NewViewModel(IWarriorSkill model) => _factory(model);
}
