﻿// This file is automatically generated

#nullable enable

using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;


public partial interface IBattleConfigService : IModelService<BattleConfig> {}

public partial class BattleConfigService : BaseDataModelService<BattleConfig>, IBattleConfigService
{
    public static BattleConfigService Load(string dataFile) => new BattleConfigService(dataFile);
    private BattleConfigService(string dataFile) : base(dataFile, 0, 46, () => new BattleConfig(), 47) {}

    public BattleConfigService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.BattleConfigRomPath)) {}

    public BattleConfig Retrieve(BattleConfigId id) => Retrieve((int)id);

    public override string IdToName(int id)
    {
        return ((BattleConfigId)id).ToString();
    }
}
