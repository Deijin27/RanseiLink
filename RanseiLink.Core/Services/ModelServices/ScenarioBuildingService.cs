using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IScenarioBuildingService : IModelService<ScenarioBuilding>
    {
    }

    public class ScenarioBuildingService : BaseScenarioModelService<ScenarioBuilding>, IScenarioBuildingService
    {
        public ScenarioBuildingService(ModInfo mod) : base(mod.FolderPath, 0, 10)
        {
        }

        public ScenarioBuilding Retrieve(ScenarioId id) => Retrieve((int)id);

        protected override string IdToRelativePath(int id)
        {
            return Constants.ScenarioBuildingPathFromId(id);
        }

        public override string IdToName(int id)
        {
            return ((ScenarioId)id).ToString();
        }
    }
}
