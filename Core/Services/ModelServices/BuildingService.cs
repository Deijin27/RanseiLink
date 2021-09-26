using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;

namespace Core.Services.ModelServices
{
    public interface IBuildingService : IModelDataService<BuildingId, IBuilding>
    {
        IDisposableBuildingService Disposable();
    }

    public interface IDisposableBuildingService : IDisposableModelDataService<BuildingId, IBuilding>
    {
    }

    public class BuildingService : BaseModelService, IBuildingService
    {
        public BuildingService(ModInfo mod) : base(mod, Constants.BuildingRomPath, Building.DataLength) { }

        public IDisposableBuildingService Disposable()
        {
            return new DisposableBuildingService(Mod);
        }

        public IBuilding Retrieve(BuildingId id)
        {
            return new Building(RetrieveData((int)id));
        }

        public void Save(BuildingId id, IBuilding model)
        {
            SaveData((int)id, model.Data);
        }
    }

    public class DisposableBuildingService : BaseDisposableModelService, IDisposableBuildingService
    {
        public DisposableBuildingService(ModInfo mod) : base(mod, Constants.BuildingRomPath, Building.DataLength) { }

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
