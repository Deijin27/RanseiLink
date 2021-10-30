using Core.Enums;
using Core.Models;
using Core.Models.Interfaces;

namespace Core.Services.ModelServices
{
    public interface IScenarioAppearPokemonService : IModelDataService<ScenarioId, IScenarioAppearPokemon>
    {
        IDisposableScenarioAppearPokemonService Disposable();
    }

    public interface IDisposableScenarioAppearPokemonService : IDisposableModelDataService<ScenarioId, IScenarioAppearPokemon>
    {
    }

    public class ScenarioAppearPokemonService : BaseScenarioService, IScenarioAppearPokemonService
    {
        public ScenarioAppearPokemonService(ModInfo mod) : base(mod, ScenarioAppearPokemon.DataLength, 0, Constants.ScenarioAppearPokemonPathFromId)
        {

        }

        public IDisposableScenarioAppearPokemonService Disposable()
        {
            return new DisposableScenarioAppearPokemonService(Mod);
        }

        public IScenarioAppearPokemon Retrieve(ScenarioId scenario)
        {
            return new ScenarioAppearPokemon(RetrieveData(scenario, 0));
        }

        public void Save(ScenarioId scenario, IScenarioAppearPokemon model)
        {
            SaveData(scenario, 0, model.Data);
        }
    }

    public class DisposableScenarioAppearPokemonService : BaseDisposableScenarioService, IDisposableScenarioAppearPokemonService
    {
        public DisposableScenarioAppearPokemonService(ModInfo mod) : base(mod, ScenarioAppearPokemon.DataLength, 0, Constants.ScenarioAppearPokemonPathFromId)
        {

        }

        public IScenarioAppearPokemon Retrieve(ScenarioId scenario)
        {
            return new ScenarioAppearPokemon(RetrieveData(scenario, 0));
        }

        public void Save(ScenarioId scenario, IScenarioAppearPokemon model)
        {
            SaveData(scenario, 0, model.Data);
        }
    }
}
