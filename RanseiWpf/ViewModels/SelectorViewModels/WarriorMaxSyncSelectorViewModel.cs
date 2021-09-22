using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;

namespace RanseiWpf.ViewModels
{
    public class WarriorMaxSyncSelectorViewModel : SelectorViewModelBase<WarriorId, IWarriorMaxLink, WarriorMaxSyncViewModel>
    {
        public WarriorMaxSyncSelectorViewModel(WarriorId initialSelected, IModelDataService<WarriorId, IWarriorMaxLink> dataService) : base(initialSelected, dataService) { }
    }
}
