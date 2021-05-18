using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;

namespace RanseiWpf.ViewModels
{
    public class AbilitySelectorViewModel : SelectorViewModelBase<AbilityId, IAbility, AbilityViewModel>
    {
        public AbilitySelectorViewModel(AbilityId initialSelected, IModelDataService<AbilityId, IAbility> dataService) : base(initialSelected, dataService) { }
    }
}
