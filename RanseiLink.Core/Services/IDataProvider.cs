using FluentResults;
using RanseiLink.Core.Enums;
using System;
using System.Collections.Generic;

namespace RanseiLink.Core.Services
{
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
        Title
    }

    public class SpriteFile
    {
        public SpriteType Type { get; }
        public int Id { get; } 
        public string File { get; }
        public bool IsOverride { get; }
        public SpriteFile(SpriteType type, int id, string file, bool isOverride)
        {
            Type = type;
            Id = id;
            File = file;
            IsOverride = isOverride;
        }
    }

    public class DataFile
    {
        public string RomPath { get; }
        public string File { get; }
        public bool IsOverride { get; }
        public DataFile(string romPath, string file, bool isOverride)
        {
            RomPath = romPath;
            File = file;
            IsOverride = isOverride;
        }
    }

    public interface IFallbackDataProvider
    {
        bool IsDefaultsPopulated(ConquestGameCode gc);
        Result Populate(string ndsFile, IProgress<ProgressInfo> progress = null);
        List<SpriteFile> GetAllSpriteFiles(ConquestGameCode gc, SpriteType type);
        SpriteFile GetSpriteFile(ConquestGameCode gc, SpriteType type, int id);
        DataFile GetDataFile(ConquestGameCode gc, string pathInRom);
        List<DataFile> GetAllDataFilesInFolder(ConquestGameCode gc, string pathOfFolderInRom);
    }

    public class SpriteModifiedArgs
    {
        public SpriteType Type { get; }
        public int Id { get; }
        public SpriteModifiedArgs(SpriteType type, int id)
        {
            Type = type;
            Id = id;
        }   
    }

    public interface IOverrideDataProvider
    {
        event EventHandler<SpriteModifiedArgs> SpriteModified;
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
}