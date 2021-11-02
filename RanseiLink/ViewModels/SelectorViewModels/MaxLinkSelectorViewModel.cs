using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Linq;

namespace RanseiLink.ViewModels
{
    public class MaxLinkSelectorViewModel : SelectorViewModelBase<WarriorId, IMaxLink, MaxLinkViewModel>
    {
        public MaxLinkSelectorViewModel(
            IDialogService dialogService,
            WarriorId initialSelected,
            IModelDataService<WarriorId, IMaxLink> dataService)
        : base(dialogService, initialSelected, dataService, EnumUtil.GetValuesExceptDefaults<WarriorId>().ToArray()) { }
    }
}
