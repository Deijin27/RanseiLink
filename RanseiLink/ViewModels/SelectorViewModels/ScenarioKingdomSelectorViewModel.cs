using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public delegate ScenarioKingdomSelectorViewModel ScenarioKingdomSelectorViewModelFactory(IEditorContext context);

public class ScenarioKingdomSelectorViewModel : SelectorViewModelBase<ScenarioId, IScenarioKingdom, ScenarioKingdomViewModel>
{
    private readonly ScenarioKingdomViewModelFactory _factory;
    public ScenarioKingdomSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.ScenarioKingdom) 
    {
        _factory = container.Resolve<ScenarioKingdomViewModelFactory>();
        Selected = ScenarioId.TheLegendOfRansei;
    }

    protected override ScenarioKingdomViewModel NewViewModel(IScenarioKingdom model) => _factory(model);
}
