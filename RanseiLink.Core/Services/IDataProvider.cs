using FluentResults;
using RanseiLink.Core.Enums;
using System;
using System.Collections.Generic;

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
    ModelPokemon,
    StlPokemonB,
    StlPokemonCI,
    StlPokemonL,
    StlPokemonM,
    StlPokemonS,
    StlPokemonSR,
    StlPokemonWu,
    StlStaffRoll,
    StlStageObje,
    Title,
    Minimap,
    IconInstS,
    Castlemap,
    KuniImage,
    KuniImage2
}

/// <summary>
/// A snapshot of the state of a sprite data file
/// </summary>
/// <param name="Type"></param>
/// <param name="Id"></param>
/// <param name="RomPath">Path of file relative to the root of the rom</param>
/// <param name="File">Absolute path of the file in the local file system</param>
/// <param name="IsOverride">True if the file is overwritten</param>
public record SpriteFile(SpriteType Type, int Id, string RomPath, string File, bool IsOverride) : DataFile(RomPath, File, IsOverride);

/// <summary>
/// A snapshot of the state of a data file
/// </summary>
/// <param name="RomPath">Path of file relative to the root of the rom</param>
/// <param name="File">Absolute path of the file in the local file system</param>
/// <param name="IsOverride">True if the file is overwritten</param>
public record DataFile(string RomPath, string File, bool IsOverride);

public interface IFallbackDataProvider
{
    bool IsDefaultsPopulated(ConquestGameCode gc);
    Result Populate(string ndsFile, IProgress<ProgressInfo>? progress = null);
    List<SpriteFile> GetAllSpriteFiles(ConquestGameCode gc, SpriteType type);
    SpriteFile GetSpriteFile(ConquestGameCode gc, SpriteType type, int id);
    DataFile GetDataFile(ConquestGameCode gc, string pathInRom);
    List<DataFile> GetAllDataFilesInFolder(ConquestGameCode gc, string pathOfFolderInRom);
}

public record SpriteModifiedArgs(SpriteType Type, int Id);

public interface IOverrideDataProvider
{
    event EventHandler<SpriteModifiedArgs>? SpriteModified;
    bool IsDefaultsPopulated();
    void SetOverride(SpriteType type, int id, string file);
    void ClearOverride(SpriteType type, int id);
    List<SpriteFile> GetAllSpriteFiles(SpriteType type);
    SpriteFile GetSpriteFile(SpriteType type, int id);

    void SetOverride(string pathInRom, string file);
    void ClearOverride(string pathInRom);
    DataFile GetDataFile(string pathInRom);
    List<DataFile> GetAllDataFilesInFolder(string pathOfFolderInRom);

}