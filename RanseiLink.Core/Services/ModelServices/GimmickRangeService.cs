using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;

public interface IGimmickRangeService : IModelDataService<GimmickRangeId, IMoveRange>
{
    IDisposableGimmickRangeService Disposable();
}

public interface IDisposableGimmickRangeService : IDisposableModelDataService<GimmickRangeId, IMoveRange>
{
}

public class GimmickRangeService : BaseModelService, IGimmickRangeService
{
    public GimmickRangeService(ModInfo mod) : base(mod, Constants.GimmickRangeRomPath, MoveRange.DataLength, 29) { }

    public IDisposableGimmickRangeService Disposable()
    {
        return new DisposableGimmickRangeService(Mod);
    }

    public IMoveRange Retrieve(GimmickRangeId id)
    {
        return new MoveRange(RetrieveData((int)id));
    }

    public void Save(GimmickRangeId id, IMoveRange model)
    {
        SaveData((int)id, model.Data);
    }
}

public class DisposableGimmickRangeService : BaseDisposableModelService, IDisposableGimmickRangeService
{
    public DisposableGimmickRangeService(ModInfo mod) : base(mod, Constants.GimmickRangeRomPath, MoveRange.DataLength, 29) { }

    public IMoveRange Retrieve(GimmickRangeId id)
    {
        return new MoveRange(RetrieveData((int)id));
    }

    public void Save(GimmickRangeId id, IMoveRange model)
    {
        SaveData((int)id, model.Data);
    }
}
