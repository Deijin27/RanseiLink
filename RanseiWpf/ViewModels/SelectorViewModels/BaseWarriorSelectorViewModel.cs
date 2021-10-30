using Core;
using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;
using RanseiWpf.Services;
using System.Linq;

namespace RanseiWpf.ViewModels
{
    public class BaseWarriorSelectorViewModel : SelectorViewModelBase<WarriorId, IBaseWarrior, BaseWarriorViewModel>
    {
        public BaseWarriorSelectorViewModel(IDialogService dialogService, WarriorId initialSelected, IModelDataService<WarriorId, IBaseWarrior> dataService) 
            : base(dialogService, initialSelected, dataService, EnumUtil.GetValuesExceptDefaults<WarriorId>().ToArray()) { }
    }
}
