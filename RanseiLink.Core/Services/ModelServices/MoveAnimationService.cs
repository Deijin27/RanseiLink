using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;

public interface IMoveAnimationService : IModelDataService<MoveAnimationId, IMoveAnimation>
{
    IDisposableMoveAnimationService Disposable();
}

public interface IDisposableMoveAnimationService : IDisposableModelDataService<MoveAnimationId, IMoveAnimation>
{
}

public class MoveAnimationService : BaseModelService, IMoveAnimationService
{
    public MoveAnimationService(ModInfo mod) : base(mod, Constants.MoveEffectRomPath, MoveAnimation.DataLength, 254) { }

    public IDisposableMoveAnimationService Disposable()
    {
        return new DisposableMoveAnimationService(Mod);
    }

    public IMoveAnimation Retrieve(MoveAnimationId id)
    {
        return new MoveAnimation(RetrieveData((int)id));
    }

    public void Save(MoveAnimationId id, IMoveAnimation model)
    {
        SaveData((int)id, model.Data);
    }
}

public class DisposableMoveAnimationService : BaseDisposableModelService, IDisposableMoveAnimationService
{
    public DisposableMoveAnimationService(ModInfo mod) : base(mod, Constants.MoveEffectRomPath, MoveAnimation.DataLength, 254) { }

    public IMoveAnimation Retrieve(MoveAnimationId id)
    {
        return new MoveAnimation(RetrieveData((int)id));
    }

    public void Save(MoveAnimationId id, IMoveAnimation model)
    {
        SaveData((int)id, model.Data);
    }
}
