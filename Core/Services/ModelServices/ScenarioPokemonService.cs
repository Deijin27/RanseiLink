using Core.Enums;
using Core.Models;
using Core.Models.Interfaces;

namespace Core.Services.ModelServices
{
    public interface IScenarioPokemonService : IModelDataService<ScenarioId, int, IScenarioPokemon>
    {
        IDisposableScenarioPokemonService Disposable();
    }

    public interface IDisposableScenarioPokemonService : IDisposableModelDataService<ScenarioId, int, IScenarioPokemon>
    {
    }

    public class ScenarioPokemonService : BaseScenarioService, IScenarioPokemonService
    {
        public ScenarioPokemonService(ModInfo mod) : base(mod, ScenarioPokemon.DataLength, Constants.ScenarioPokemonCount - 1, Constants.ScenarioPokemonPathFromId)
        {

        }

        public IDisposableScenarioPokemonService Disposable()
        {
            return new DisposableScenarioPokemonService(Mod);
        }

        public IScenarioPokemon Retrieve(ScenarioId scenario, int id)
        {
            return new ScenarioPokemon(RetrieveData(scenario, id));
        }

        public void Save(ScenarioId scenario, int id, IScenarioPokemon model)
        {
            SaveData(scenario, id, model.Data);
        }
    }

    public class DisposableScenarioPokemonService : BaseDisposableScenarioService, IDisposableScenarioPokemonService
    {
        public DisposableScenarioPokemonService(ModInfo mod) : base(mod, ScenarioPokemon.DataLength, Constants.ScenarioPokemonCount - 1, Constants.ScenarioPokemonPathFromId)
        {

        }

        public IScenarioPokemon Retrieve(ScenarioId scenario, int id)
        {
            return new ScenarioPokemon(RetrieveData(scenario, id));
        }

        public void Save(ScenarioId scenario, int id, IScenarioPokemon model)
        {
            SaveData(scenario, id, model.Data);
        }
    }
}
