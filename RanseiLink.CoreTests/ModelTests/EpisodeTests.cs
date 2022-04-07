using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using Xunit;

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

        Assert.Equal(EpisodeId.TheLegendOfRansei, a.UnlockCondition);
        Assert.Equal(1u, a.Difficulty);
        Assert.Equal(12u, a.Order);
        Assert.Equal(ScenarioId.ShingenVsKenshin, a.Scenario);
    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        Episode a = new Episode
        {
            UnlockCondition = EpisodeId.ADateWithDestiny,
            Difficulty = 4u,
            Order = 10u,
            Scenario = ScenarioId.UniteRansei
        };

        Assert.Equal(EpisodeId.ADateWithDestiny, a.UnlockCondition);
        Assert.Equal(4u, a.Difficulty);
        Assert.Equal(10u, a.Order);
        Assert.Equal(ScenarioId.UniteRansei, a.Scenario);
    }
}
