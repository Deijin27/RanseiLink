using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;
using RanseiWpf.Services;

namespace RanseiWpf.ViewModels
{
    public class WarriorSkillSelectorViewModel : SelectorViewModelBase<WarriorSkillId, IWarriorSkill, WarriorSkillViewModel>
    {
        public WarriorSkillSelectorViewModel(IDialogService dialogService, WarriorSkillId initialSelected, IModelDataService<WarriorSkillId, IWarriorSkill> dataService) 
            : base(dialogService, initialSelected, dataService) { }
    }
}
