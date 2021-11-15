using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Linq;

namespace RanseiLink.ViewModels;

public class AbilitySelectorViewModel : SelectorViewModelBase<AbilityId, IAbility, AbilityViewModel>
{
    public AbilitySelectorViewModel(IDialogService dialogService, AbilityId initialSelected, IModelDataService<AbilityId, IAbility> dataService)
        : base(dialogService, initialSelected, dataService, EnumUtil.GetValuesExceptDefaults<AbilityId>().ToArray()) { }
}
