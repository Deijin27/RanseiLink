using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IScenarioKingdomService : IModelService<ScenarioKingdom>
    {
    }

    public class ScenarioKingdomService : BaseScenarioModelService<ScenarioKingdom>, IScenarioKingdomService
    {
        public ScenarioKingdomService(ModInfo mod) : base(mod.FolderPath, 0, 10)
        {
        }

        public ScenarioKingdom Retrieve(ScenarioId id) => Retrieve((int)id);

        protected override string IdToRelativePath(int id)
        {
            return Constants.ScenarioKingdomPathFromId(id);
        }

        public override string IdToName(int id)
        {
            return ((ScenarioId)id).ToString();
        }
    }
}