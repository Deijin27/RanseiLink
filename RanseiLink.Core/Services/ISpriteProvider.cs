﻿using RanseiLink.Core.Nds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RanseiLink.Core.Services;

public enum SpriteType
{
    StlBushouB,
    StlBushouCI,
    StlBushouF,
    StlBushouLL,
    StlBushouM,
    StlBushouS,
    StlBushouWu,
    StlCastleIcon,
    StlChikei,
    StlEventCommon,
    StlEventBg,
    StlPokemonB,
    StlPokemonCI,
    StlPokemonL,
    StlPokemonM,
    StlPokemonS,
    StlPokemonSR,
    StlPokemonWu,
    StlStaffRoll,
    StlStageObje,
}

public interface ISpriteProvider
{
    List<SpriteFile> GetAllSpriteFiles(SpriteType type);
    string GetSpriteFilePath(SpriteType type, uint id);
}

public record SpriteFile(SpriteType Type, uint Id, string File, bool IsOverride);


public interface IFallbackSpriteProvider : ISpriteProvider
{
    public void Populate(string ndsFile, IProgress<ProgressInfo> progress = null);
}

public interface IOverrideSpriteProvider : ISpriteProvider
{
    public void SetOverride(SpriteType type, uint id, string file);
    public void ClearOverride(SpriteType type, uint id);
}