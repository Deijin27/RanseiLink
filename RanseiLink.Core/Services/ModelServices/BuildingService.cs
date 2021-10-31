using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices
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
        public BuildingService(ModInfo mod) : base(mod, Constants.BuildingRomPath, Building.DataLength, 118) { }

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
        public DisposableBuildingService(ModInfo mod) : base(mod, Constants.BuildingRomPath, Building.DataLength, 118) { }

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
