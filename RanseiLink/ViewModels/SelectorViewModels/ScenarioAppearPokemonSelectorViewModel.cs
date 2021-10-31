using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels
{
    public class ScenarioAppearPokemonSelectorViewModel : SelectorViewModelBase<ScenarioId, IScenarioAppearPokemon, ScenarioAppearPokemonViewModel>
    {
        public ScenarioAppearPokemonSelectorViewModel(IDialogService dialogService, ScenarioId initialSelected, IModelDataService<ScenarioId, IScenarioAppearPokemon> dataService) 
            : base(dialogService, initialSelected, dataService) { }
    }
}
