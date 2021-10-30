using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;
using RanseiWpf.Services;

namespace RanseiWpf.ViewModels
{
    public class ScenarioKingdomSelectorViewModel : SelectorViewModelBase<ScenarioId, IScenarioKingdom, ScenarioKingdomViewModel>
    {
        public ScenarioKingdomSelectorViewModel(IDialogService dialogService, ScenarioId initialSelected, IModelDataService<ScenarioId, IScenarioKingdom> dataService) 
            : base(dialogService, initialSelected, dataService) { }
    }
}
