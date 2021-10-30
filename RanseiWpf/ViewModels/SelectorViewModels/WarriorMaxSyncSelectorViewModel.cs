using Core;
using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;
using RanseiWpf.Services;
using System.Linq;

namespace RanseiWpf.ViewModels
{
    public class WarriorMaxSyncSelectorViewModel : SelectorViewModelBase<WarriorId, IWarriorMaxLink, WarriorMaxSyncViewModel>
    {
        public WarriorMaxSyncSelectorViewModel(
            IDialogService dialogService,
            WarriorId initialSelected,
            IModelDataService<WarriorId, IWarriorMaxLink> dataService)
        : base(dialogService, initialSelected, dataService, EnumUtil.GetValuesExceptDefaults<WarriorId>().ToArray()) { }
    }
}
