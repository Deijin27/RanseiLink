using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Models.Interfaces;
using System.IO;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IScenarioKingdomService : IModelDataService<ScenarioId, IScenarioKingdom>
    {
        IDisposableScenarioKingdomService Disposable();
    }

    public interface IDisposableScenarioKingdomService : IDisposableModelDataService<ScenarioId, IScenarioKingdom>
    {
    }

    public class ScenarioKingdomService : BaseScenarioService, IScenarioKingdomService
    {
        public ScenarioKingdomService(ModInfo mod) : base(mod, ScenarioKingdom.DataLength, 0, Constants.ScenarioKingdomPathFromId)
        {

        }

        public IDisposableScenarioKingdomService Disposable()
        {
            return new DisposableScenarioKingdomService(Mod);
        }

        public IScenarioKingdom Retrieve(ScenarioId scenario)
        {
            return new ScenarioKingdom(RetrieveData(scenario, 0));
        }

        public void Save(ScenarioId scenario, IScenarioKingdom model)
        {
            SaveData(scenario, 0, model.Data);
        }
    }

    public class DisposableScenarioKingdomService : BaseDisposableScenarioService, IDisposableScenarioKingdomService
    {
        public DisposableScenarioKingdomService(ModInfo mod) : base(mod, ScenarioKingdom.DataLength, 0, Constants.ScenarioKingdomPathFromId)
        {

        }

        public IScenarioKingdom Retrieve(ScenarioId scenario)
        {
            return new ScenarioKingdom(RetrieveData(scenario, 0));
        }

        public void Save(ScenarioId scenario, IScenarioKingdom model)
        {
            SaveData(scenario, 0, model.Data);
        }
    }
}
