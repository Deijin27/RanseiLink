using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels
{
    public class WarriorSkillSelectorViewModel : SelectorViewModelBase<WarriorSkillId, IWarriorSkill, WarriorSkillViewModel>
    {
        public WarriorSkillSelectorViewModel(IDialogService dialogService, WarriorSkillId initialSelected, IModelDataService<WarriorSkillId, IWarriorSkill> dataService) 
            : base(dialogService, initialSelected, dataService) { }
    }
}
