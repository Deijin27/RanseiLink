using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;

public interface IGimmickService : IModelDataService<GimmickId, IGimmick>
{
    IDisposableGimmickService Disposable();
}

public interface IDisposableGimmickService : IDisposableModelDataService<GimmickId, IGimmick>
{
}

public class GimmickService : BaseModelService, IGimmickService
{
    public GimmickService(ModInfo mod) : base(mod, Constants.GimmickRomPath, Gimmick.DataLength, 147) { }

    public IDisposableGimmickService Disposable()
    {
        return new DisposableGimmickService(Mod);
    }

    public IGimmick Retrieve(GimmickId id)
    {
        return new Gimmick(RetrieveData((int)id));
    }

    public void Save(GimmickId id, IGimmick model)
    {
        SaveData((int)id, model.Data);
    }
}

public class DisposableGimmickService : BaseDisposableModelService, IDisposableGimmickService
{
    public DisposableGimmickService(ModInfo mod) : base(mod, Constants.GimmickRomPath, Gimmick.DataLength, 147) { }

    public IGimmick Retrieve(GimmickId id)
    {
        return new Gimmick(RetrieveData((int)id));
    }

    public void Save(GimmickId id, IGimmick model)
    {
        SaveData((int)id, model.Data);
    }
}
