using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;

public interface IGimmickObjectService : IModelDataService<GimmickObjectId, IGimmickObject>
{
    IDisposableGimmickObjectService Disposable();
}

public interface IDisposableGimmickObjectService : IDisposableModelDataService<GimmickObjectId, IGimmickObject>
{
}

public class GimmickObjectService : BaseModelService, IGimmickObjectService
{
    public GimmickObjectService(ModInfo mod) : base(mod, Constants.GimmickObjectRomPath, GimmickObject.DataLength, 99) { }

    public IDisposableGimmickObjectService Disposable()
    {
        return new DisposableGimmickObjectService(Mod);
    }

    public IGimmickObject Retrieve(GimmickObjectId id)
    {
        return new GimmickObject(RetrieveData((int)id));
    }

    public void Save(GimmickObjectId id, IGimmickObject model)
    {
        SaveData((int)id, model.Data);
    }
}

public class DisposableGimmickObjectService : BaseDisposableModelService, IDisposableGimmickObjectService
{
    public DisposableGimmickObjectService(ModInfo mod) : base(mod, Constants.GimmickObjectRomPath, GimmickObject.DataLength, 99) { }

    public IGimmickObject Retrieve(GimmickObjectId id)
    {
        return new GimmickObject(RetrieveData((int)id));
    }

    public void Save(GimmickObjectId id, IGimmickObject model)
    {
        SaveData((int)id, model.Data);
    }
}
