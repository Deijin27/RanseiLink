using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Models;
using RanseiLink.Core.Graphics;
using Xunit;
using FluentAssertions;

namespace RanseiLink.CoreTests.ModelTests;

public class BattleConfigTests
{
    [Fact]
    public void AccessorsReturnCorrectValues()
    {
        BattleConfig a = new BattleConfig(new byte[]
        {
            0x01, 0xB8, 0x68, 0x7C, 0x4D, 0x80, 0x33, 0x02, 0x00, 0x80, 0xA4, 0x14, 0x00, 0x00, 0x00, 0x00, 0x07, 0x00, 0x0E, 0x00, 0x0E, 0x00, 0x00, 0x00
        });

        a.MapId.Should().Be(new MapId(1, 0));
        a.UpperAtmosphereColor.Should().Be(new Rgb15(23, 8, 3));
        a.MiddleAtmosphereColor.Should().Be(new Rgb15(13, 2, 0));
        a.LowerAtmosphereColor.Should().Be(new Rgb15(7, 3, 1));
        a.VictoryCondition.Should().Be(BattleVictoryConditionFlags.DefeatAllEnemies);
        a.DefeatCondition.Should().Be(BattleVictoryConditionFlags.DefeatAllEnemies);
        a.NumberOfTurns.Should().Be(20);
    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        BattleConfig a = new BattleConfig()
        {
            MapId = new MapId(10, 1),
            UpperAtmosphereColor = new Rgb15(9, 7, 21),
            MiddleAtmosphereColor = new Rgb15(20, 19, 16),
            LowerAtmosphereColor = new Rgb15(12, 11, 1),
            VictoryCondition = BattleVictoryConditionFlags.ClaimAllBanners | BattleVictoryConditionFlags.HoldAllBannersFor5Turns,
            DefeatCondition = BattleVictoryConditionFlags.ClaimAllBanners,
            NumberOfTurns = 17
        };

        a.MapId.Should().Be(new MapId(10, 1));
        a.UpperAtmosphereColor.Should().Be(new Rgb15(9, 7, 21));
        a.MiddleAtmosphereColor.Should().Be(new Rgb15(20, 19, 16));
        a.LowerAtmosphereColor.Should().Be(new Rgb15(12, 11, 1));
        a.VictoryCondition.Should().Be(BattleVictoryConditionFlags.ClaimAllBanners | BattleVictoryConditionFlags.HoldAllBannersFor5Turns);
        a.DefeatCondition.Should().Be(BattleVictoryConditionFlags.ClaimAllBanners);
        a.NumberOfTurns.Should().Be(17);
    }
}
