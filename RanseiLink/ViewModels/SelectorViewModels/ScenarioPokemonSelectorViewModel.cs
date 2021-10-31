using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System;

namespace RanseiLink.ViewModels
{
    public class ScenarioPokemonSelectorViewModel : ScenarioSelectorViewModelBase<IScenarioPokemon, ScenarioPokemonViewModel>
    {
        private readonly IModelDataService<ScenarioId, int, IScenarioPokemon> Service;
        public ScenarioPokemonSelectorViewModel(IDialogService dialogService, IModelDataService<ScenarioId, int, IScenarioPokemon> service, Func<ScenarioId, ScenarioPokemonViewModel> newVm)
            : base(dialogService, newVm, 0, 199)
        {
            Service = service;
            Init();
        }

        protected override IScenarioPokemon RetrieveModel(ScenarioId scenario, uint index)
        {
            return Service.Retrieve(scenario, (int)index);
        }

        protected override void SaveModel(ScenarioId scenario, uint index, IScenarioPokemon model)
        {
            Service.Save(scenario, (int)index, model);
        }
    }
}
