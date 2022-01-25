using RanseiLink.Core.Nds;
using System;

namespace RanseiLink.Core.Services;

[Flags]
public enum SpriteExportOptions
{
    None = 0,
    IncludePaintNetPalette = 1
}

public interface ISpriteService
{
    void ExportAllPokemonSprites(INds rom, string desinationFolder, SpriteExportOptions options = 0);
}
