using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Linq;

namespace RanseiLink.ViewModels;

public class BaseWarriorSelectorViewModel : SelectorViewModelBase<WarriorId, IBaseWarrior, BaseWarriorViewModel>
{
    public BaseWarriorSelectorViewModel(IDialogService dialogService, WarriorId initialSelected, IModelDataService<WarriorId, IBaseWarrior> dataService)
        : base(dialogService, initialSelected, dataService, EnumUtil.GetValuesExceptDefaults<WarriorId>().ToArray()) { }
}
