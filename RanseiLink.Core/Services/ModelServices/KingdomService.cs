using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;

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
