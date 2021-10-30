using Core;
using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;
using RanseiWpf.Services;
using System.Linq;

namespace RanseiWpf.ViewModels
{
    public class AbilitySelectorViewModel : SelectorViewModelBase<AbilityId, IAbility, AbilityViewModel>
    {
        public AbilitySelectorViewModel(IDialogService dialogService, AbilityId initialSelected, IModelDataService<AbilityId, IAbility> dataService) 
            : base(dialogService, initialSelected, dataService, EnumUtil.GetValuesExceptDefaults<AbilityId>().ToArray()) { }
    }
}
