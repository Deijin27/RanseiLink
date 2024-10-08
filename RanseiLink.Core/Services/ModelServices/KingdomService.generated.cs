﻿// This file is automatically generated

#nullable enable

using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;


public partial interface IKingdomService : IModelService<Kingdom> {}

public partial class KingdomService : BaseDataModelService<Kingdom>, IKingdomService
{
    public static KingdomService Load(string dataFile, ConquestGameCode culture) => new KingdomService(dataFile, culture);
    private KingdomService(string dataFile, ConquestGameCode culture) : base(dataFile, 0, 16, () => new Kingdom(culture), 17) {}

    public KingdomService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.KingdomRomPath), mod.GameCode) {}

    public Kingdom Retrieve(KingdomId id) => Retrieve((int)id);

    public override string IdToName(int id)
    {
        return Retrieve(id).Name;
    }
}
