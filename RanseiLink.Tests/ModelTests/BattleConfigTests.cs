using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Models;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Graphics;
using Xunit;

namespace RanseiLink.Tests.ModelTests;

public class BattleConfigTests
{
    [Fact]
    public void AccessorsReturnCorrectValues()
    {
        IBattleConfig a = new BattleConfig(new byte[]
        {
            0x01, 0xB8, 0x68, 0x7C, 0x4D, 0x80, 0x33, 0x02, 0x00, 0x80, 0xA4, 0x14, 0x00, 0x00, 0x00, 0x00, 0x07, 0x00, 0x0E, 0x00, 0x0E, 0x00, 0x00, 0x00
        });

        Assert.Equal(new MapId(1, 0), a.MapId);
        Assert.Equal(new Rgb15(23, 8, 3), a.UpperAtmosphereColor);
        Assert.Equal(new Rgb15(13, 2, 0), a.MiddleAtmosphereColor);
        Assert.Equal(new Rgb15(7, 3, 1), a.LowerAtmosphereColor);
        Assert.Equal(BattleVictoryConditionFlags.DefeatAllEnemies, a.VictoryCondition);
        Assert.Equal(BattleVictoryConditionFlags.DefeatAllEnemies, a.DefeatCondition);
        Assert.Equal(20u, a.NumberOfTurns);
    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        IBattleConfig a = new BattleConfig()
        {
            MapId = new MapId(10, 1),
            UpperAtmosphereColor = new Rgb15(9, 7, 21),
            MiddleAtmosphereColor = new Rgb15(20, 19, 16),
            LowerAtmosphereColor = new Rgb15(12, 11, 1),
            VictoryCondition = BattleVictoryConditionFlags.ClaimAllBanners | BattleVictoryConditionFlags.HoldAllBannersFor5Turns,
            DefeatCondition = BattleVictoryConditionFlags.ClaimAllBanners,
            NumberOfTurns = 17
        };

        Assert.Equal(new MapId(10, 1), a.MapId);
        Assert.Equal(new Rgb15(9, 7, 21), a.UpperAtmosphereColor);
        Assert.Equal(new Rgb15(20, 19, 16), a.MiddleAtmosphereColor);
        Assert.Equal(new Rgb15(12, 11, 1), a.LowerAtmosphereColor);
        Assert.Equal(BattleVictoryConditionFlags.ClaimAllBanners | BattleVictoryConditionFlags.HoldAllBannersFor5Turns, a.VictoryCondition);
        Assert.Equal(BattleVictoryConditionFlags.ClaimAllBanners, a.DefeatCondition);
        Assert.Equal(17u, a.NumberOfTurns);
    }
}
