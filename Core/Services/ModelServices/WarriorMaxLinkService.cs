using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;

namespace Core.Services.ModelServices
{
    public interface IWarriorMaxLinkService : IModelDataService<WarriorId, IWarriorMaxLink>
    {
        IDisposableWarriorMaxLinkService Disposable();
    }

    public interface IDisposableWarriorMaxLinkService : IDisposableModelDataService<WarriorId, IWarriorMaxLink>
    {
    }

    public class WarriorMaxLinkService : BaseModelService, IWarriorMaxLinkService
    {
        public WarriorMaxLinkService(ModInfo mod) : base(mod, Constants.BaseBushouMaxSyncTableRomPath, WarriorMaxLink.DataLength, 251) { }

        public IDisposableWarriorMaxLinkService Disposable()
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

    public class DisposableWarriorMaxSyncService : BaseDisposableModelService, IDisposableWarriorMaxLinkService
    {
        public DisposableWarriorMaxSyncService(ModInfo mod) : base(mod, Constants.BaseBushouMaxSyncTableRomPath, WarriorMaxLink.DataLength, 251)
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
