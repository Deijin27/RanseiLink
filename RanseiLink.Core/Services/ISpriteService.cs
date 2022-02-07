using RanseiLink.Core.Graphics;
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
    void ExtractStl(string stlDataFile, NCER ncer, string outputFolder);
}

[Flags]
public enum StlFile
{
    stl_busho_b = 1 << 0,
    stl_busho_ci = 1 << 1,
    stl_busho_f = 1 << 2,
    stl_busho_ll = 1 << 3,
    stl_busho_m = 1 << 4,
    stl_busho_s = 1 << 5,
    stl_busho_wu = 1 << 6,
    stl_castleicon = 1 << 7,
    stl_chikei = 1 << 8,
    stl_event = 1 << 9,
    stl_pokemon_b = 1 << 10,
    stl_pokemon_ci = 1 << 11,
    stl_pokemon_l = 1 << 12,
    stl_pokemon_m = 1 << 13,
    stl_pokemon_s = 1 << 14,
    stl_pokemon_sr = 1 << 15,
    stl_pokemon_wu = 1 << 16,
    stl_staffroll = 1 << 17,
    stl_stageobje = 1 << 18,
    type = 1 << 19,

    all_busho = stl_busho_b | stl_busho_ci | stl_busho_f | stl_busho_ll | stl_busho_m | stl_busho_s | stl_busho_wu,
    all_pokemon = stl_pokemon_b | stl_pokemon_ci | stl_pokemon_l | stl_pokemon_m | stl_pokemon_s | stl_pokemon_sr | stl_pokemon_wu,
    all = all_busho | stl_castleicon | stl_chikei | stl_event | all_pokemon | stl_staffroll | stl_stageobje | type,
}