using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Linq;

namespace RanseiLink.ViewModels
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
