using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using Xunit;

namespace RanseiLink.CoreTests.ModelTests;

public class BuildingTests
{
    [Fact]
    public void AccessorsReturnCorrectValues()
    {
        Building b = new Building(new byte[]
        {
                0x53, 0x61, 0x63, 0x72,
                0x65, 0x64, 0x20, 0x52,
                0x75, 0x69, 0x6E, 0x73,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x77,
                0x77, 0x77, 0x77, 0x05,
                0x04, 0x03, 0x77, 0x00,
                0x14, 0x0A, 0x00, 0x00,
                0x14, 0x26, 0x93, 0x99
        });

        Assert.Equal("Sacred Ruins", b.Name);
        Assert.Equal(KingdomId.Aurora, b.Kingdom);
        Assert.Equal(BattleConfigId.SacredRuins, b.BattleConfig1);
        Assert.Equal(BattleConfigId.SacredRuins, b.BattleConfig2);
        Assert.Equal(BattleConfigId.SacredRuins, b.BattleConfig3);
        Assert.Equal(BuildingSpriteId.SacredRuins, b.Sprite1);
        Assert.Equal(BuildingSpriteId.SacredRuins, b.Sprite2);
        Assert.Equal(BuildingSpriteId.SacredRuins, b.Sprite3);
        Assert.Equal(BuildingFunctionId.Battle, b.Function);

    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        Building b = new Building
        {
            Name = "Sacred Ruins",
            Kingdom = KingdomId.Cragspur,
            BattleConfig1 = BattleConfigId.Cragspur,
            BattleConfig2 = BattleConfigId.SkyGarden_1,
            BattleConfig3 = BattleConfigId.Terrera,
            Sprite1 = BuildingSpriteId.SnowyMountain_1,
            Sprite2 = BuildingSpriteId.UndergroundMine_1,
            Sprite3 = BuildingSpriteId.Ravine_2,
            Function = BuildingFunctionId.ObtainGold
        };

        Assert.Equal("Sacred Ruins", b.Name);
        Assert.Equal(KingdomId.Cragspur, b.Kingdom);
        Assert.Equal(BattleConfigId.Cragspur, b.BattleConfig1);
        Assert.Equal(BattleConfigId.SkyGarden_1, b.BattleConfig2);
        Assert.Equal(BattleConfigId.Terrera, b.BattleConfig3);
        Assert.Equal(BuildingSpriteId.SnowyMountain_1, b.Sprite1);
        Assert.Equal(BuildingSpriteId.UndergroundMine_1, b.Sprite2);
        Assert.Equal(BuildingSpriteId.Ravine_2, b.Sprite3);
        Assert.Equal(BuildingFunctionId.ObtainGold, b.Function);

        // Add Array equal test when possible
    }
}
