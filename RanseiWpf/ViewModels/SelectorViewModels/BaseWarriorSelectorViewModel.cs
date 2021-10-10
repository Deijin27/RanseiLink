using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;

namespace RanseiWpf.ViewModels
{
    public class BaseWarriorSelectorViewModel : SelectorViewModelBase<WarriorId, IBaseWarrior, BaseWarriorViewModel>
    {
        public BaseWarriorSelectorViewModel(WarriorId initialSelected, IModelDataService<WarriorId, IBaseWarrior> dataService) : base(initialSelected, dataService) { }
    }
}
