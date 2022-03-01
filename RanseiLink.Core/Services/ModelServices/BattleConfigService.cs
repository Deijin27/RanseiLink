using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;

public interface IBattleConfigService : IModelDataService<BattleConfigId, IBattleConfig>
{
    IDisposableBattleConfigService Disposable();
}

public interface IDisposableBattleConfigService : IDisposableModelDataService<BattleConfigId, IBattleConfig>
{
}

public class BattleConfigService : BaseModelService, IBattleConfigService
{
    public BattleConfigService(ModInfo mod) : base(mod, Constants.BattleConfigRomPath, BattleConfig.DataLength, 46) { }

    public IDisposableBattleConfigService Disposable()
    {
        return new DisposableBattleConfigService(Mod);
    }

    public IBattleConfig Retrieve(BattleConfigId id)
    {
        return new BattleConfig(RetrieveData((int)id));
    }

    public void Save(BattleConfigId id, IBattleConfig model)
    {
        SaveData((int)id, model.Data);
    }
}

public class DisposableBattleConfigService : BaseDisposableModelService, IDisposableBattleConfigService
{
    public DisposableBattleConfigService(ModInfo mod) : base(mod, Constants.BattleConfigRomPath, BattleConfig.DataLength, 46) { }

    public IBattleConfig Retrieve(BattleConfigId id)
    {
        return new BattleConfig(RetrieveData((int)id));
    }

    public void Save(BattleConfigId id, IBattleConfig model)
    {
        SaveData((int)id, model.Data);
    }
}
