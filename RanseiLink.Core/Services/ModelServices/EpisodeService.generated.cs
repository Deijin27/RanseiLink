﻿// This file is automatically generated

#nullable enable

using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;


public partial interface IEpisodeService : IModelService<Episode> {}

public partial class EpisodeService : BaseDataModelService<Episode>, IEpisodeService
{
    public static EpisodeService Load(string dataFile) => new EpisodeService(dataFile);
    private EpisodeService(string dataFile) : base(dataFile, 0, 37, () => new Episode(), 511) {}

    public EpisodeService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.EpisodeRomPath)) {}

    public Episode Retrieve(EpisodeId id) => Retrieve((int)id);

    public override string IdToName(int id)
    {
        return ((EpisodeId)id).ToString();
    }
}