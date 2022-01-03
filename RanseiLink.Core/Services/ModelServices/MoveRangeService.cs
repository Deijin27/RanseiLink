using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;

public interface IMoveRangeService : IModelDataService<MoveRangeId, IMoveRange>
{
    IDisposableMoveRangeService Disposable();
}

public interface IDisposableMoveRangeService : IDisposableModelDataService<MoveRangeId, IMoveRange>
{
}

public class MoveRangeService : BaseModelService, IMoveRangeService
{
    public MoveRangeService(ModInfo mod) : base(mod, Constants.MoveRangeRomPath, MoveRange.DataLength, 29) { }

    public IDisposableMoveRangeService Disposable()
    {
        return new DisposableMoveRangeService(Mod);
    }

    public IMoveRange Retrieve(MoveRangeId id)
    {
        return new MoveRange(RetrieveData((int)id));
    }

    public void Save(MoveRangeId id, IMoveRange model)
    {
        SaveData((int)id, model.Data);
    }
}

public class DisposableMoveRangeService : BaseDisposableModelService, IDisposableMoveRangeService
{
    public DisposableMoveRangeService(ModInfo mod) : base(mod, Constants.MoveRangeRomPath, MoveRange.DataLength, 29) { }

    public IMoveRange Retrieve(MoveRangeId id)
    {
        return new MoveRange(RetrieveData((int)id));
    }

    public void Save(MoveRangeId id, IMoveRange model)
    {
        SaveData((int)id, model.Data);
    }
}
