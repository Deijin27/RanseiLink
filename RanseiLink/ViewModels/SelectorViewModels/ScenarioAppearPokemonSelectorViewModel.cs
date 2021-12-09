using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public delegate ScenarioAppearPokemonSelectorViewModel ScenarioAppearPokemonSelectorViewModelFactory(IEditorContext context);

public class ScenarioAppearPokemonSelectorViewModel : SelectorViewModelBase<ScenarioId, IScenarioAppearPokemon, ScenarioAppearPokemonViewModel>
{
    private readonly ScenarioAppearPokemonViewModelFactory _factory;
    public ScenarioAppearPokemonSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.ScenarioAppearPokemon) 
    {
        _factory = container.Resolve<ScenarioAppearPokemonViewModelFactory>();
        Selected = ScenarioId.TheLegendOfRansei;
    }

    protected override ScenarioAppearPokemonViewModel NewViewModel(IScenarioAppearPokemon model) => _factory(model);
}
