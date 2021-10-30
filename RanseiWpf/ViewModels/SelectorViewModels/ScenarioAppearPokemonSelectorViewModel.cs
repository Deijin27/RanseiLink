using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;
using RanseiWpf.Services;

namespace RanseiWpf.ViewModels
{
    public class ScenarioAppearPokemonSelectorViewModel : SelectorViewModelBase<ScenarioId, IScenarioAppearPokemon, ScenarioAppearPokemonViewModel>
    {
        public ScenarioAppearPokemonSelectorViewModel(IDialogService dialogService, ScenarioId initialSelected, IModelDataService<ScenarioId, IScenarioAppearPokemon> dataService) 
            : base(dialogService, initialSelected, dataService) { }
    }
}
