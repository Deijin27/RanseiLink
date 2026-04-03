using System.ComponentModel;

namespace RanseiLink.Core.Enums;


[Flags]
public enum BattleVictoryConditionFlags
{
    [Description("Hold All Banners For 5 Turns")]
    HoldAllBannersFor5Turns = 0b1,
    [Description("Claim All Banners")]
    ClaimAllBanners = 0b10,
    [Description("Unknown 0b100")]
    Unknown_0b100 = 0b100,
    [Description("Unknown 0b1000")]
    Unknown_0b1000 = 0b1000,
    [Description("Unknown 0b10000")]
    Unknown_0b10000 = 0b10000,
}