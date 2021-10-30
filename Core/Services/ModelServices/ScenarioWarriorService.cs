using Core.Enums;
using Core.Models;
using Core.Models.Interfaces;

namespace Core.Services.ModelServices
{
    public interface IScenarioWarriorService : IModelDataService<ScenarioId, int, IScenarioWarrior>
    {
        IDisposableScenarioWarriorService Disposable();
    }

    public interface IDisposableScenarioWarriorService : IDisposableModelDataService<ScenarioId, int, IScenarioWarrior>
    {
    }

    public class ScenarioWarriorService : BaseScenarioService, IScenarioWarriorService
    {
        public ScenarioWarriorService(ModInfo mod) : base(mod, ScenarioWarrior.DataLength, Constants.ScenarioWarriorCount - 1, Constants.ScenarioWarriorPathFromId)
        {

        }

        public IDisposableScenarioWarriorService Disposable()
        {
            return new DisposableScenarioWarriorService(Mod);
        }

        public IScenarioWarrior Retrieve(ScenarioId scenario, int id)
        {
            return new ScenarioWarrior(RetrieveData(scenario, id));
        }

        public void Save(ScenarioId scenario, int id, IScenarioWarrior model)
        {
            SaveData(scenario, id, model.Data);
        }
    }

    public class DisposableScenarioWarriorService : BaseDisposableScenarioService, IDisposableScenarioWarriorService
    {
        public DisposableScenarioWarriorService(ModInfo mod) : base(mod, ScenarioWarrior.DataLength, Constants.ScenarioWarriorCount - 1, Constants.ScenarioWarriorPathFromId)
        {

        }

        public IScenarioWarrior Retrieve(ScenarioId scenario, int id)
        {
            return new ScenarioWarrior(RetrieveData(scenario, id));
        }

        public void Save(ScenarioId scenario, int id, IScenarioWarrior model)
        {
            SaveData(scenario, id, model.Data);
        }
    }
}
