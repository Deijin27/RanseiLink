using System;

namespace RanseiLink.Core.Enums;

[Flags]
public enum BattleVictoryConditionFlags : uint
{
    DefeatAllEnemies       = 0b_00000,
    Unknown_Aurora         = 0b_00001,
    Unknown_ViperiaDragnor = 0b_00010,
    Unknown_Greenleaf      = 0b_00100,
    Unknown_Pugilis        = 0b_01000, 
    ClaimAllBanners        = 0b_10000,
}
