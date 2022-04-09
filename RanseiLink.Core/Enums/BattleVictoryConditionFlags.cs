using System;

namespace RanseiLink.Core.Enums;

[Flags]
public enum BattleVictoryConditionFlags
{
    DefeatAllEnemies        = 0b_00000,
    Unknown_AuroraDragnor   = 0b_00001,
    Unknown_ViperiaDragnor  = 0b_00010,
    Unknown_Greenleaf       = 0b_00100,
    HoldAllBannersFor5Turns = 0b_01000, 
    ClaimAllBanners         = 0b_10000,
}
