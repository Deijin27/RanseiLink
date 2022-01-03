using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;

public interface IGimmickRangeService : IModelDataService<GimmickRangeId, IAttackRange>
{
    IDisposableGimmickRangeService Disposable();
}

public interface IDisposableGimmickRangeService : IDisposableModelDataService<GimmickRangeId, IAttackRange>
{
}

public class GimmickRangeService : BaseModelService, IGimmickRangeService
{
    public GimmickRangeService(ModInfo mod) : base(mod, Constants.GimmickRangeRomPath, AttackRange.DataLength, 29) { }

    public IDisposableGimmickRangeService Disposable()
    {
        return new DisposableGimmickRangeService(Mod);
    }

    public IAttackRange Retrieve(GimmickRangeId id)
    {
        return new AttackRange(RetrieveData((int)id));
    }

    public void Save(GimmickRangeId id, IAttackRange model)
    {
        SaveData((int)id, model.Data);
    }
}

public class DisposableGimmickRangeService : BaseDisposableModelService, IDisposableGimmickRangeService
{
    public DisposableGimmickRangeService(ModInfo mod) : base(mod, Constants.GimmickRangeRomPath, AttackRange.DataLength, 29) { }

    public IAttackRange Retrieve(GimmickRangeId id)
    {
        return new AttackRange(RetrieveData((int)id));
    }

    public void Save(GimmickRangeId id, IAttackRange model)
    {
        SaveData((int)id, model.Data);
    }
}
