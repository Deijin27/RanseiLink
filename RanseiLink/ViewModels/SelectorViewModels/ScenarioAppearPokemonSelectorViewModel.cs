using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.ViewModels;

public delegate ScenarioAppearPokemonSelectorViewModel ScenarioAppearPokemonSelectorViewModelFactory(IScenarioAppearPokemonService service);

public class ScenarioAppearPokemonSelectorViewModel : SelectorViewModelBase<ScenarioId, IScenarioAppearPokemon, ScenarioAppearPokemonViewModel>
{
    private readonly ScenarioAppearPokemonViewModelFactory _factory;
    public ScenarioAppearPokemonSelectorViewModel(IServiceContainer container, IScenarioAppearPokemonService dataService)
        : base(container, dataService) 
    {
        _factory = container.Resolve<ScenarioAppearPokemonViewModelFactory>();
        Selected = ScenarioId.TheLegendOfRansei;
    }

    protected override ScenarioAppearPokemonViewModel NewViewModel(IScenarioAppearPokemon model) => _factory(model);
}
