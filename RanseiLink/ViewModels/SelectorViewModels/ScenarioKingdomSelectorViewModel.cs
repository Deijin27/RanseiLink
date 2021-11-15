using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public class ScenarioKingdomSelectorViewModel : SelectorViewModelBase<ScenarioId, IScenarioKingdom, ScenarioKingdomViewModel>
{
    public ScenarioKingdomSelectorViewModel(IDialogService dialogService, ScenarioId initialSelected, IModelDataService<ScenarioId, IScenarioKingdom> dataService)
        : base(dialogService, initialSelected, dataService) { }
}
