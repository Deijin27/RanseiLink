using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;

public interface IMoveRangeService : IModelDataService<MoveRangeId, IAttackRange>
{
    IDisposableMoveRangeService Disposable();
}

public interface IDisposableMoveRangeService : IDisposableModelDataService<MoveRangeId, IAttackRange>
{
}

public class MoveRangeService : BaseModelService, IMoveRangeService
{
    public MoveRangeService(ModInfo mod) : base(mod, Constants.MoveRangeRomPath, AttackRange.DataLength, 29) { }

    public IDisposableMoveRangeService Disposable()
    {
        return new DisposableMoveRangeService(Mod);
    }

    public IAttackRange Retrieve(MoveRangeId id)
    {
        return new AttackRange(RetrieveData((int)id));
    }

    public void Save(MoveRangeId id, IAttackRange model)
    {
        SaveData((int)id, model.Data);
    }
}

public class DisposableMoveRangeService : BaseDisposableModelService, IDisposableMoveRangeService
{
    public DisposableMoveRangeService(ModInfo mod) : base(mod, Constants.MoveRangeRomPath, AttackRange.DataLength, 29) { }

    public IAttackRange Retrieve(MoveRangeId id)
    {
        return new AttackRange(RetrieveData((int)id));
    }

    public void Save(MoveRangeId id, IAttackRange model)
    {
        SaveData((int)id, model.Data);
    }
}
