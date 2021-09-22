using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;

namespace Core.Services.ModelServices
{
    public class BuildingService : BaseModelService, IModelDataService<BuildingId, IBuilding>
    {
        public BuildingService(ModInfo mod) : base(mod, Constants.BuildingRomPath, Building.DataLength) { }

        public IBuilding Retrieve(BuildingId id)
        {
            return new Building(RetrieveData((int)id));
        }

        public void Save(BuildingId id, IBuilding model)
        {
            SaveData((int)id, model.Data);
        }
    }
}
