using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;

namespace RanseiWpf.ViewModels
{
    public class WarriorSkillSelectorViewModel : SelectorViewModelBase<WarriorSkillId, IWarriorSkill, WarriorSkillViewModel>
    {
        public WarriorSkillSelectorViewModel(WarriorSkillId initialSelected, IModelDataService<WarriorSkillId, IWarriorSkill> dataService) : base(initialSelected, dataService) { }
    }
}
