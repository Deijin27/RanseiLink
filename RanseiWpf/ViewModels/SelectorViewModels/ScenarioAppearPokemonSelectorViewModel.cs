using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;

namespace RanseiWpf.ViewModels
{
    public class ScenarioAppearPokemonSelectorViewModel : SelectorViewModelBase<ScenarioId, IScenarioAppearPokemon, ScenarioAppearPokemonViewModel>
    {
        public ScenarioAppearPokemonSelectorViewModel(ScenarioId initialSelected, IModelDataService<ScenarioId, IScenarioAppearPokemon> dataService) : base(initialSelected, dataService) { }
    }
}
