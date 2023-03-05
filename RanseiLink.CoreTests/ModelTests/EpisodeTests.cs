using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.CoreTests.ModelTests;

public class EpisodeTests
{
    [Fact]
    public void AccessorsReturnCorrectValues()
    {
        // the joy of battle
        Episode a = new Episode(new byte[]
        {
            0x0C, 0x0A, 0xD4, 0x3E, 
            0x40, 0xFE, 0xFF, 0x13
        });

        a.UnlockCondition.Should().Be(EpisodeId.TheLegendOfRansei);
        a.Difficulty.Should().Be(1);
        a.Order.Should().Be(12);
        a.Scenario.Should().Be(ScenarioId.ShingenVsKenshin);
    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        Episode a = new Episode
        {
            UnlockCondition = EpisodeId.ADateWithDestiny,
            Difficulty = 4,
            Order = 10,
            Scenario = ScenarioId.UniteRansei
        };

        a.UnlockCondition.Should().Be(EpisodeId.ADateWithDestiny);
        a.Difficulty.Should().Be(4);
        a.Order.Should().Be(10);
        a.Scenario.Should().Be(ScenarioId.UniteRansei);
    }
}
