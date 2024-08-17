using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;

public interface IBattleConfigService : IModelService<BattleConfig>
{
}

public class BattleConfigService : BaseNewableDataModelService<BattleConfig>, IBattleConfigService
{
    private BattleConfigService(string BattleConfigDatFile) : base(BattleConfigDatFile, 0, 46, 47) { }

    public BattleConfigService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.BattleConfigRomPath)) { }

    public BattleConfig Retrieve(BattleConfigId id) => Retrieve((int)id);

    public override string IdToName(int id)
    {
        return ((BattleConfigId)id).ToString();
    }
}