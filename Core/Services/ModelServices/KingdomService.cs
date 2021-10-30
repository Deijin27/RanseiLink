using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;

namespace Core.Services.ModelServices
{
    public interface IKingdomService : IModelDataService<KingdomId, IKingdom>
    {
        IDisposableKingdomService Disposable();
    }

    public interface IDisposableKingdomService : IDisposableModelDataService<KingdomId, IKingdom>
    {
    }

    public class KingdomService : BaseModelService, IKingdomService
    {
        public KingdomService(ModInfo mod) : base(mod, Constants.KingdomRomPath, Kingdom.DataLength, 16) { }

        public IDisposableKingdomService Disposable()
        {
            return new DisposableKingdomService(Mod);
        }

        public IKingdom Retrieve(KingdomId id)
        {
            return new Kingdom(RetrieveData((int)id));
        }

        public void Save(KingdomId id, IKingdom model)
        {
            SaveData((int)id, model.Data);
        }
    }

    public class DisposableKingdomService : BaseDisposableModelService, IDisposableKingdomService
    {
        public DisposableKingdomService(ModInfo mod) : base(mod, Constants.KingdomRomPath, Kingdom.DataLength, 16) { }

        public IKingdom Retrieve(KingdomId id)
        {
            return new Kingdom(RetrieveData((int)id));
        }

        public void Save(KingdomId id, IKingdom model)
        {
            SaveData((int)id, model.Data);
        }
    }
}
