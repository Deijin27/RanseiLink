using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IMaxLinkService : IModelDataService<WarriorId, IMaxLink>
    {
        IDisposableMaxLinkService Disposable();
    }

    public interface IDisposableMaxLinkService : IDisposableModelDataService<WarriorId, IMaxLink>
    {
    }

    public class MaxLinkService : BaseModelService, IMaxLinkService
    {
        public MaxLinkService(ModInfo mod) : base(mod, Constants.BaseBushouMaxSyncTableRomPath, MaxLink.DataLength, 251) { }

        public IDisposableMaxLinkService Disposable()
        {
            return new DisposableMaxLinkService(Mod);
        }

        public IMaxLink Retrieve(WarriorId id)
        {
            return new MaxLink(RetrieveData((int)id));
        }

        public void Save(WarriorId id, IMaxLink model)
        {
            SaveData((int)id, model.Data);
        }
    }

    public class DisposableMaxLinkService : BaseDisposableModelService, IDisposableMaxLinkService
    {
        public DisposableMaxLinkService(ModInfo mod) : base(mod, Constants.BaseBushouMaxSyncTableRomPath, MaxLink.DataLength, 251)
        {
        }

        public IMaxLink Retrieve(WarriorId id)
        {
            return new MaxLink(RetrieveData((int)id));
        }

        public void Save(WarriorId id, IMaxLink model)
        {
            SaveData((int)id, model.Data);
        }
    }
}
