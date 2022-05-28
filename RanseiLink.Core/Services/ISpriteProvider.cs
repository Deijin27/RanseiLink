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

    public interface ISpriteProvider
    {
        List<SpriteFile> GetAllSpriteFiles(SpriteType type);
        SpriteFile GetSpriteFile(SpriteType type, int id);
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
    


    public interface IFallbackSpriteProvider : ISpriteProvider
    {
        bool IsDefaultsPopulated { get; }
        void Populate(string ndsFile, IProgress<ProgressInfo> progress = null);
    }

    public interface IOverrideSpriteProvider : ISpriteProvider
    {
        void SetOverride(SpriteType type, int id, string file);
        void ClearOverride(SpriteType type, int id);
    }
}