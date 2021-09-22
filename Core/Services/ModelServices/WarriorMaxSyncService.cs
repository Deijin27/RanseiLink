using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;

namespace Core.Services.ModelServices
{
    public interface IWarriorMaxSyncService : IModelDataService<WarriorId, IWarriorMaxLink>
    {
        IDisposableWarriorMaxSyncService Disposable();
    }

    public interface IDisposableWarriorMaxSyncService : IDisposableModelDataService<WarriorId, IWarriorMaxLink>
    {
    }

    public class WarriorMaxSyncService : BaseModelService, IWarriorMaxSyncService
    {
        public WarriorMaxSyncService(ModInfo mod) : base(mod, Constants.BaseBushouMaxSyncTableRomPath, WarriorMaxLink.DataLength) { }

        public IDisposableWarriorMaxSyncService Disposable()
        {
            return new DisposableWarriorMaxSyncService(Mod);
        }

        public IWarriorMaxLink Retrieve(WarriorId id)
        {
            return new WarriorMaxLink(RetrieveData((int)id));
        }

        public void Save(WarriorId id, IWarriorMaxLink model)
        {
            SaveData((int)id, model.Data);
        }
    }

    public class DisposableWarriorMaxSyncService : BaseDisposableModelService, IDisposableWarriorMaxSyncService
    {
        public DisposableWarriorMaxSyncService(ModInfo mod) : base(mod, Constants.BaseBushouMaxSyncTableRomPath, WarriorMaxLink.DataLength)
        {
        }

        public IWarriorMaxLink Retrieve(WarriorId id)
        {
            return new WarriorMaxLink(RetrieveData((int)id));
        }

        public void Save(WarriorId id, IWarriorMaxLink model)
        {
            SaveData((int)id, model.Data);
        }
    }
}
