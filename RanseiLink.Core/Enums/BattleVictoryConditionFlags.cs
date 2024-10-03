using System.ComponentModel;

namespace RanseiLink.Core.Enums;


[Flags]
public enum BattleVictoryConditionFlags
{
    [Description("Defeat All Enemies")]
    DefeatAllEnemies = 0b_00000,
    [Description("Unknown 1 (Dragnor/Aurora)")]
    Unknown_AuroraDragnor = 0b_00001,
    [Description("Unknown 2 (Dragnor/Viperia)")]
    Unknown_ViperiaDragnor = 0b_00010,
    [Description("Unknown 3 (Greenleaf)")]
    Unknown_Greenleaf = 0b_00100,
    [Description("Hold All Banners For 5 Turns")]
    HoldAllBannersFor5Turns = 0b_01000,
    [Description("Claim All Banners")]
    ClaimAllBanners = 0b_10000,
}