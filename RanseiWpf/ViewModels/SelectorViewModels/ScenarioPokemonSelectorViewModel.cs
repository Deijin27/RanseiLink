using Core.Models.Interfaces;
using Core.Services;

namespace RanseiWpf.ViewModels
{
    public class ScenarioPokemonSelectorViewModel : ScenarioSelectorViewModelBase<IScenarioPokemon, ScenarioPokemonViewModel>
    {
        private readonly IModelDataService<int, int, IScenarioPokemon> Service;
        public ScenarioPokemonSelectorViewModel(IModelDataService<int, int, IScenarioPokemon> service) : base(0, 199)
        {
            Service = service;
            Init();
        }

        protected override IScenarioPokemon RetrieveModel(uint scenario, uint index)
        {
            return Service.Retrieve((int)scenario, (int)index);
        }

        protected override void SaveModel(uint scenario, uint index, IScenarioPokemon model)
        {
            Service.Save((int)scenario, (int)index, model);
        }
    }
}
