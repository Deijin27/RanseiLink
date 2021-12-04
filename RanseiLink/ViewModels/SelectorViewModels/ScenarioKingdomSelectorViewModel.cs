using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.ViewModels;

public delegate ScenarioKingdomSelectorViewModel ScenarioKingdomSelectorViewModelFactory(IScenarioKingdomService service);

public class ScenarioKingdomSelectorViewModel : SelectorViewModelBase<ScenarioId, IScenarioKingdom, ScenarioKingdomViewModel>
{
    private readonly ScenarioKingdomViewModelFactory _factory;
    public ScenarioKingdomSelectorViewModel(IServiceContainer container, IScenarioKingdomService dataService)
        : base(container, dataService) 
    {
        _factory = container.Resolve<ScenarioKingdomViewModelFactory>();
        Selected = ScenarioId.TheLegendOfRansei;
    }

    protected override ScenarioKingdomViewModel NewViewModel(IScenarioKingdom model) => _factory(model);
}
